#!/usr/bin/env python3

# The face cards are not part of this system and are simply scoured from their original source

import os
import lxml.etree as ET
import copy
import sys
import subprocess

base = ET.parse(sys.argv[1])
svg = "{http://www.w3.org/2000/svg}"
inkscape = "{http://www.inkscape.org/namespaces/inkscape}"

suits = (
    ("spade", ["#000000"]),
    ("heart", ["#DF0000"]),
    ("diamo", ["#DF3800", "#DF8300"]),
    ("clubs", ["#1F331F", "#073FB2"]),
)
ranks = (
    ["ace"],                      # Ace
    ["2-3"],                      # 2
    ["2-3", "3-5"],               # 3
    ["4-10"],                     # 4
    ["4-10", "3-5"],              # 5
    ["4-10", "6-8"],              # 6
    ["4-10", "6-8", "7-8"],       # 7
    ["4-10", "6-8", "7-8", "8"],  # 8
    ["4-10", "9-10"],             # 9
    ["4-10", "9-10", "10"],       # 10
)

def write_card(card, filename):
    card.write(f"{filename}.tmp.svg")
    subprocess.run(("scour", "--enable-id-stripping", "--strip-xml-space",
                    f"{filename}.tmp.svg", f"{filename}.svg"))
    os.remove(f"{filename}.tmp.svg")

def set_color(card, color):
    for group in card.getroot().iterfind(".//{*}path"):
        group.attrib["fill"] = color

for suit, colors in suits:
    for rank, pips in enumerate(ranks, 1):
        card = copy.deepcopy(base)

        for group in card.getroot().iterfind("{*}g"):
            label = group.get(inkscape + "label")
            if label == "ranks":
                rank_group = group
            if label == "pips":
                pips_group = group
            if label == "rank-pips":
                rank_pip_group = group
        for subgroup in rank_group:
            if subgroup.get(inkscape + "label") != "rank" + str(rank):
                subgroup.getparent().remove(subgroup)
        for subgroup in rank_pip_group:
            if subgroup.get(inkscape + "label") != suit:
                subgroup.getparent().remove(subgroup)
        for subgroup in pips_group:
            if subgroup.get(inkscape + "label") not in pips:
                subgroup.getparent().remove(subgroup)
            else:
                for suitgroup in subgroup:
                    if suitgroup.get(inkscape + "label") != suit:
                        suitgroup.getparent().remove(suitgroup)

        for elem in card.findall(".//*"):
            if elem.tag == svg + "rect":
                continue
            if "fill" in elem.attrib:
                del elem.attrib["fill"]
            if "style" in elem.attrib:
                del elem.attrib["style"]

        set_color(card, colors[0])
        write_card(card, f"{suit}{rank}")
        if len(colors) == 2:
            set_color(card, colors[1])
            write_card(card, f"{suit}{rank}-alt")
