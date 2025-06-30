using Desktop;
using SPNATI_Character_Editor.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Character), 215, DelayRun = true, Caption = "Images")]
	[Activity(typeof(Costume), 215, DelayRun = true, Caption = "Images")]
	public partial class ScreenshotTaker : Activity
	{
		private ISkin _character;
//		private Character _characterC;
		private Dictionary<string, string> _extraData = new Dictionary<string, string>();

		private readonly Image EmptyImage = new Bitmap(1, 1);

		public ScreenshotTaker()
		{
			InitializeComponent();

			cboBoxPoseConversion.Items.AddRange(new string[] { "Replace Poses with Custom Poses", "Replace Poses with Pose Sets", "Replace Custom Poses with Pose Sets", "Replace Custom Poses with Poses", "Replace Pose Sets with Custom Poses", "Replace Pose Sets with Poses" });
		}

		public override string Caption
		{
			get { return "Images"; }
		}

		protected override void OnInitialize()
		{
			_character = Record as ISkin;
		}

		protected override void OnActivate()
		{
			PopulateFileList();
		}

		private void PopulateFileList()
		{
			gridFiles.Rows.Clear();
			string dir = _character.GetDirectory();
			DirectoryInfo directory = new DirectoryInfo(dir);
			foreach (FileInfo file in directory.EnumerateFiles()
				.Where(f => f.Extension == ".png"))
			{
				DataGridViewRow row = gridFiles.Rows[gridFiles.Rows.Add()];
				row.Cells[0].Value = file.Name;
				row.Cells[1].Value = ToKB(file.Length);
				row.Cells[2].Value = IsCompressed(file.Name) ? Properties.Resources.Checkmark : EmptyImage;
			}
		}

		private string ToKB(long length)
		{
			double size = length / 1024.0;
			return $"{Math.Round(size, 2)} KB";
		}

		static readonly byte[] PngPreamble = {
			// Signature
			0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
			// Beginning of IHDR Chunk
			0x00, 0x00, 0x00, 0x0D,  // Length: 13
			0x49, 0x48, 0x44, 0x52,  // Type: IHDR
		};

		private bool IsCompressed(string filename)
		{
			string dir = _character.GetDirectory();
			string path = Path.Combine(dir, filename);
			if (!File.Exists(path))
			{
				return false;
			}

			using (FileStream stream = File.Open(path, FileMode.Open))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					byte[] preamble = reader.ReadBytes(PngPreamble.Length);
					if (!PngPreamble.SequenceEqual(preamble))
					{
						// invalid
						return false;
					}

					reader.ReadBytes(8);  // Skip image width and height
					reader.ReadByte();  // Skip bit depth, assumed 8 bits per channel

					byte colorType = reader.ReadByte();
					return colorType == 3;  // Indexed color
				}
			}
		}

		private void cmdImport_Click(object sender, EventArgs e)
		{
			string file = Path.GetFileNameWithoutExtension(txtName.Text);
			if (string.IsNullOrEmpty(file))
			{
				MessageBox.Show("File name is blank.", "Import Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			ImageCropper cropper = new ImageCropper();
			cropper.ImportUnprocessed(_extraData);

			if (cropper.ShowDialog() == DialogResult.OK)
			{
				Image importedImage = cropper.CroppedImage;
				if (importedImage != null)
				{
					SaveImage(file, importedImage);
					Shell.Instance.SetStatus($"{Path.Combine(_character.GetDirectory(), file + ".png")} created.");
				}
			}
		}

		private void SaveImage(string imageKey, Image image)
		{
			string filename = imageKey + ".png";
			string fullPath = Path.Combine(_character.GetDirectory(), filename);

			try
			{
				string compressionPath = Path.Combine(_character.GetBackupDirectory(), "images", filename);
				if (File.Exists(compressionPath))
				{
					File.Delete(compressionPath);
				}

				image.Save(fullPath);
			}
			catch (Exception ex)
			{
				ErrorLog.LogError(ex.ToString());
			}
			PopulateFileList();
		}

		private void cmdAdvanced_Click(object sender, EventArgs e)
		{
			PoseSettingsForm form = new PoseSettingsForm();
			form.SetData(_extraData);
			if (form.ShowDialog() == DialogResult.OK)
			{
				_extraData = form.GetData();
			}
		}

		private void cmdCompressAll_Click(object sender, EventArgs e)
		{
			List<string> images = CompileCompressionList((file, row) =>
			{
				return !IsCompressed(file);
			});
			Compress(images);
		}

		private void cmdCompressSelected_Click(object sender, EventArgs e)
		{
			List<string> images = CompileCompressionList((file, row) =>
			{
				return row.Selected;
			});
			Compress(images);
		}

		private List<string> CompileCompressionList(Func<string, DataGridViewRow, bool> filter)
		{
			List<string> output = new List<string>();
			for (int i = 0; i < gridFiles.Rows.Count; i++)
			{
				DataGridViewRow row = gridFiles.Rows[i];
				string file = row.Cells[0].Value?.ToString();
				if (string.IsNullOrEmpty(file) || !filter(file, row))
				{
					continue;
				}
				output.Add(file);
			}
			return output;
		}

		private void Compress(List<string> files)
		{
			ProgressForm progressForm = new ProgressForm();
			progressForm.Text = "Compress Images";
			progressForm.Show(this);

			int maxCount = files.Count;
			var progressUpdate = new Progress<int>(value => progressForm.SetProgress(string.Format("Compressing {0}...", files[value]), value, maxCount));

			progressForm.Shown += async (s, args) =>
			{
				var cts = new CancellationTokenSource();
				progressForm.SetCancellationSource(cts);
				try
				{
					int result = await CompressAsync(progressUpdate, files, cts.Token);
					if (result < 0)
					{
						MessageBox.Show($"An error occurred during compression.", "Compress Images", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					else
					{
						Shell.Instance.SetStatus($"Compressed {maxCount} images.");
					}
				}
				finally
				{
					progressForm.Close();
				}

				PopulateFileList();
			};
		}

		private Task<int> CompressAsync(IProgress<int> progress, List<string> files, CancellationToken ct)
		{
			return Task.Run(() =>
			{
				try
				{
					IImageCompressor compressor = new TinifyCompressor();
					string dir = _character.GetDirectory();
					int current = 0;
					bool hasErrors = false;
					foreach (string file in files)
					{
						progress.Report(current++);

						string fullPath = Path.Combine(dir, file);
						if (!compressor.Compress(fullPath, _character))
						{
							break;
						}

						ct.ThrowIfCancellationRequested();
					}

					return hasErrors ? -1 : 1;
				}
				catch (OperationCanceledException)
				{
					return 0;
				}
			}, ct);
		}
		private void cboBoxPoseConversion_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedArg = cboBoxPoseConversion.SelectedItem.ToString();

			switch (selectedArg)
			{
				case "Replace Poses with Custom Poses":
					labelModeDescription.Text = "Replaces pose references with custom pose equivalents, if a cross-stage pose of the same name is found (doesn't work for individual-stage custom poses).";
					break;

				case "Replace Poses with Pose Sets":
					labelModeDescription.Text = "Replaces pose references with pose sets if stage coverage is complete. The pose set must have a name that matches exactly to the pose.";
					break;

				case "Replace Custom Poses with Pose Sets":
					labelModeDescription.Text = "Replaces custom pose references with a name-matching pose set.";
					break;

				case "Replace Custom Poses with Poses":
					labelModeDescription.Text = "Replaces custom pose references with name-matching regular pose images.";
					break;

				case "Replace Pose Sets with Custom Poses":
					labelModeDescription.Text = "Replaces pose set references with name-matching custom poses.";
					break;

				case "Replace Pose Sets with Poses":
					labelModeDescription.Text = "Replaces pose set references with name-matching regular pose images.";
					break;

				default:
					labelModeDescription.Text = "Select a replacement mode for more details.";
					break;
			}
		}

		enum PoseConversionMode
		{
			PosesToCustom,
			PosesToSets,
			CustomToSets,
			CustomToPoses,
			SetsToCustom,
			SetsToPoses
		}

		private PoseConversionMode GetSelectedConversionMode()
		{
			if (cboBoxPoseConversion.SelectedItem == null)
			{
				throw new InvalidOperationException("You must select a conversion mode before continuing.");
			}

			switch (cboBoxPoseConversion.SelectedItem.ToString())
			{
				case "Replace Poses with Custom Poses":
					return PoseConversionMode.PosesToCustom;
				case "Replace Poses with Pose Sets":
					return PoseConversionMode.PosesToSets;
				case "Replace Custom Poses with Pose Sets":
					return PoseConversionMode.CustomToSets;
				case "Replace Custom Poses with Poses":
					return PoseConversionMode.CustomToPoses;
				case "Replace Pose Sets with Custom Poses":
					return PoseConversionMode.SetsToCustom;
				case "Replace Pose Sets with Poses":
					return PoseConversionMode.SetsToPoses;
				default:
					throw new InvalidOperationException("Unknown conversion mode selected.");
			}
		}


		private void cmdConvertPoses_Click(object sender, EventArgs e)
		{
			try
			{
				var mode = GetSelectedConversionMode();
				RunPoseConversion(mode);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Conversion failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void RunPoseConversion(PoseConversionMode mode)
		{
			// Load pose data once
			_customPoseNames = LoadCustomPoseNames(_character);
			_poseSets = LoadPoseSets(_character);

			foreach (var name in _customPoseNames)
			{
				Debug.WriteLine($"[DEBUG] Available custom pose: {name}");
			}
			int replacements = 0;

			// Need to cast it here so that we can access .Behavior
			if (_character is Character realCharacter)
			{
				foreach (Case workingCase in realCharacter.Behavior.GetWorkingCases())
				{
					List<int> stages = workingCase.Stages;

					foreach (DialogueLine line in workingCase.Lines)
					{
						string original = line.Image;
						if (string.IsNullOrEmpty(original)) continue;

						string updated = ApplySmartReplacement(original, stages, mode);

						var newPose = _character.PoseLibrary.GetPose(updated);

						if (newPose != line.Pose)
						{
							line.Pose = newPose;
							replacements++;

							Debug.WriteLine($"Y Changed pose: {original} : {updated}");
						}
						else
						{
							Debug.WriteLine($"-> Skipped (no change): {original} : {updated}");
						}
					}
				}

				MessageBox.Show($"{replacements} pose reference(s) updated.", "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				MessageBox.Show("This tool can only be run on full character files.", "Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

		}

		private HashSet<string> _customPoseNames;
		private Dictionary<string, PoseSet> _poseSets;

		private string ApplySmartReplacement(string originalImg, List<int> caseStages, PoseConversionMode mode)
		{
			_customPoseNames = LoadCustomPoseNames(_character);
			_poseSets = LoadPoseSets(_character);
			Debug.WriteLine($"Checking: {originalImg} across stages {string.Join(",", caseStages)} | Mode: {mode}");


			if (string.IsNullOrEmpty(originalImg))
				return originalImg;

			if (IsRegularPose(originalImg, out int stage, out string poseName))
			{
				string customKey = $"{stage}-{poseName}";
				Debug.WriteLine($"-> Type matched: RegularPose ({stage}-{poseName})");

				switch (mode)
				{
					case PoseConversionMode.PosesToCustom:
						if (stage >= 0)
						{
							string key = $"{stage}-{poseName}";
							if (_customPoseNames.Contains(key))
								return $"custom:{key}";
						}

						// If stage == -1 (e.g., #-wildcard), try matching on poseName or any stage-prefixed name
						if (_customPoseNames.Contains(poseName))
							return $"custom:{poseName}";

						foreach (int s in caseStages)
						{
							string fallback = $"{s}-{poseName}";
							if (_customPoseNames.Contains(fallback))
								return $"custom:{fallback}";
						}

						break;


					case PoseConversionMode.PosesToSets:
						if (_poseSets.TryGetValue(poseName, out var set) && set.HasCoverageForStages(caseStages))
							return $"set:{poseName}";
						break;
				}
			}
			else if (IsCustomPose(originalImg, out string customName, out int? customStage))
			{
				switch (mode)
				{
					case PoseConversionMode.CustomToSets:
						if (!string.IsNullOrEmpty(customName) &&
							_poseSets.TryGetValue(customName, out var set))
						{
							// First check stage coverage
							if (!set.HasCoverageForStages(caseStages))
							{
								Debug.WriteLine($"X Pose set '{customName}' does not cover all stages: {string.Join(",", caseStages)}");
								break;
							}

							bool matchFound = false;
							foreach (int stageA in caseStages)
							{
								string stageSpecific = $"custom:{stageA}-{customName}";
								string crossStage = $"custom:{customName}";
								string wildcard = $"custom:#-{customName}";

								foreach (var entry in set.Entries)
								{
									if (entry.Img == stageSpecific || entry.Img == crossStage || entry.Img == wildcard)
									{
										matchFound = true;
										break;
									}
								}

								if (matchFound)
									break;
							}

							if (matchFound)
							{
								Debug.WriteLine($"Y Converted custom pose '{originalImg}' : set:{customName}");
								return $"set:{customName}";
							}
							else
							{
								Debug.WriteLine($"X Pose set '{customName}' does not contain a matching image for {customName} at case stages: {string.Join(",", caseStages)}");
							}
						}
						break;



					case PoseConversionMode.CustomToPoses:
						// If the custom pose has a specific stage
						if (customStage.HasValue)
						{
							string key = $"{customStage.Value}-{customName}";
							if (_character.PoseLibrary.Poses.Any(p => p.Key == key))
								return $"{key}.png";
						}

						// If the pose was cross-stage or # (wildcard), check if *each* stage used in the case has a matching pose
						foreach (int s in caseStages)
						{
							string key = $"{s}-{customName}";
							if (_character.PoseLibrary.Poses.Any(p => p.Key == key))
							{
								// If all stages in the case have the same available pose, this is a safe match
								// Optional: check for full coverage if needed
								return $"{key}.png";
							}
						}

						Debug.WriteLine($"X No matching regular pose for custom:{customName} (stages: {string.Join(",", caseStages)})");
						break;

				}
			}

			else if (IsPoseSet(originalImg, out string setName))
			{
				Debug.WriteLine($"-> Type matched: PoseSet ({setName})");

				switch (mode)
				{
					case PoseConversionMode.SetsToCustom:
						if (_customPoseNames.Contains(setName))
							return $"custom:{setName}";
						break;

					case PoseConversionMode.SetsToPoses:
						if (_poseSets.TryGetValue(setName, out var set))
						{
							var entry = set.GetPoseForStages(caseStages);
							if (entry != null)
								return entry.Img;
						}
						break;
				}
			}

			return originalImg;
		}



		bool PoseExists(string key)
		{
			return _character.PoseLibrary.Poses.Any(p => p.Key == key);
		}

			private HashSet<string> LoadCustomPoseNames(ISkin character)
		{
			HashSet<string> names = new HashSet<string>();

			foreach (var kvp in character.PoseLibrary.Poses)
			{
				string key = kvp.Key;
				if (key.StartsWith("custom:"))
				{
					// Remove the prefix for matching
					string cleanKey = key.Substring(7);
					names.Add(cleanKey);
				}
			}

			return names;
		}


		private Dictionary<string, PoseSet> LoadPoseSets(ISkin character)
		{
			return character.CustomPoseSets
				.Where(set => !string.IsNullOrEmpty(set?.Id))
				.ToDictionary(set => set.Id, set => set);
		}

		private bool IsRegularPose(string img, out int stage, out string name)
		{
			stage = -1;
			name = null;

			// Handle #-pose.png format
			if (img.StartsWith("#-"))
			{
				name = img.Substring(2, img.Length - 6); // removes "#-" and ".png"
				return true;
			}

			// Normal pose format: 4-happy.png
			var match = Regex.Match(img, @"^(\d+)-([^.]+)\.png$");
			if (match.Success)
			{
				stage = int.Parse(match.Groups[1].Value);
				name = match.Groups[2].Value;
				return true;
			}
			return false;
		}


		private bool IsCustomPose(string img, out string name, out int? stage)
		{
			name = null;
			stage = null;

			if (!img.StartsWith("custom:"))
				return false;

			string value = img.Substring(7);

			// Special case: custom:#-happy
			if (value.StartsWith("#-"))
			{
				name = value.Substring(2); // skip "#-"
				stage = null; // resolve using caseStages
				return true;
			}

			if (value.Contains("-"))
			{
				var parts = value.Split(new[] { '-' }, 2);
				if (int.TryParse(parts[0], out int parsedStage))
				{
					stage = parsedStage;
					name = parts[1];
				}
				else
				{
					// Malformed? Just return as is
					name = value;
				}
			}
			else
			{
				name = value;
			}

			return true;
		}


		bool IsPoseSet(string img, out string name)
		{
			name = null;
			if (!img.StartsWith("set:"))
				return false;

			name = img.Substring(4);
			return true;
		}

	}
}
