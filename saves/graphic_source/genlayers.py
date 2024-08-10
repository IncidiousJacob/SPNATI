#!/usr/bin/python3

import lxml.etree as ET
import copy
import sys

#ET.register_namespace('', 'http://www.w3.org/2000/svg')
svg = ET.parse(sys.stdin)

colors = [
    '#8669bc',
    '#7ea5eb',
    '#18509d', # previously #234992
    '#00a05f',
    '#ffdb39',
    '#f7b133',
    '#df6f1d',
    '#af2935',
    '#a50000',
]

for gender in ('male', 'female'):
    for n in range(1, 10):
        shirt = copy.deepcopy(svg)
        layer = shirt.find('{*}g')
        for path in layer.findall('{*}path'):
            if path.get('id') != gender + '-shirt':
                layer.remove(path)
            else:
                del path.attrib['class']
                path.attrib['fill'] = colors[n - 1]

        text = layer.find('.{*}text')
        digitspan = text.find('{*}tspan')
        digitspan.text = str(n)
        shirt.write(f"layers{n}{'f' if gender == 'female' else ''}.tmp.svg")
