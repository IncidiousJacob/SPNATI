const { app, BrowserWindow, protocol } = require("electron");
const path = require("path");

function getGameDataPath() {
  if (app.isPackaged) {
    return path.join(process.resourcesPath, "game_data");
  }

  return __dirname;
}

app.whenReady().then(() => {
  protocol.handle("spnati-data", async (request) => {
    const url = request.url.replace("spnati-data://", "");
    const filePath = path.join(getGameDataPath(), url);
    return net.fetch("file://" + filePath);
  });

  const win = new BrowserWindow({
    width: 1280,
    height: 800,
    autoHideMenuBar: true,
    webPreferences: {
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  win.loadFile(path.join(__dirname, "index.html"));
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") app.quit();
});
