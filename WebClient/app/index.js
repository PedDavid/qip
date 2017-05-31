// @flow

import 'semantic-ui-css/semantic.min.css'

import React from 'react'
import { render } from 'react-dom'

import Canvas from './scene/Board/components/Canvas'

import Pen from './model/tools/Pen'
import Eraser from './model/tools/Eraser'
import Grid from './model/Grid'

function Hello ({name}: {name: string}) {
  return <div>Hello {name}</div>
}

const grid = new Grid([], 300, 300, 0)
const pen = new Pen(grid, 'black', 4)
const eraser = new Eraser(grid, 20)
let tool = pen
const toggleTool = () => {
  if (tool === pen) {
    tool = eraser
  } else {
    tool = pen
  }
}

const listeners = {
  onDown: event => tool.onPress(event, 1),
  onUp: event => tool.onPressUp(event),
  onMove: event => tool.onSwipe(event, 1),
  onOut: event => tool.onOut(event)
}

render(
  <div>
    <Hello name='Pedro' onClick={toggleTool} />
    <button onClick={toggleTool} />
    <Canvas width={300} height={300} {...listeners} >
      HTML5 Canvas not supported
    </Canvas>
  </div>,
  document.getElementById('root')
)
