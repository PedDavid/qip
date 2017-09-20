import {SimplePoint} from './../SimplePoint'
import Tool from './Tool'

export default class Presentation implements Tool {
  constructor (grid, persist) {
    this.type = 'presentation'

    this.grid = grid
    this.persist = persist
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale, updateCanvasSizeFunc, settings) {
    if (event.buttons > 0) {
      const x = event.offsetX
      const y = event.offsetY
      const margin = {x: 0, y: 0}
      if (event.pointerType === 'touch') {
        margin.y = -100 // sets the pointer higher so the user can see
      }
      const canvasContext = event.target.getContext('2d')
      // reset all other points
      this.grid.draw(canvasContext, 1)
      // draw pointer
      Presentation._drawPointer(canvasContext, x + margin.x, y + margin.y)
      // persist pointer
      this.persist.sendPointer(new SimplePoint(x + margin.x, y + margin.y))
    }
  }

  onSwipe (event, scale, updateCanvasSizeFunc, settings) {
    if (event.buttons > 0) {
      const x = event.offsetX
      const y = event.offsetY
      const margin = {x: 0, y: 0}
      if (event.pointerType === 'touch') {
        margin.y = -100 // sets the pointer higher so the user can see
      }
      const canvasContext = event.target.getContext('2d')
      // reset all other points
      this.grid.draw(canvasContext, 1)
      // draw pointer
      Presentation._drawPointer(canvasContext, x + margin.x, y + margin.y)
      // persist pointer
      this.persist.sendPointer(new SimplePoint(x + margin.x, y + margin.y))
    }
  }

  onPressUp (event, persist) {
    const canvasContext = event.target.getContext('2d')
    this.grid.draw(canvasContext, 1) // this is necessary to reset all other points
    this.persist.sendPointer(new SimplePoint(-1, -1)) // clean last pointer in other apps
  }

  onOut (event, socket) {
    const canvasContext = event.target.getContext('2d')
    this.grid.draw(canvasContext, 1) // this is necessary to reset all other points
    this.persist.sendPointer(new SimplePoint(-1, -1)) // clean last pointer in other apps
  }

  static _drawPointer (canvasContext, x, y) {
    // outter circle
    canvasContext.beginPath()
    canvasContext.arc(x, y, 5, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = 'red'
    canvasContext.fill()
    canvasContext.lineWidth = 5
    canvasContext.strokeStyle = 'red'
    canvasContext.stroke()
    // inner circle
    canvasContext.beginPath()
    canvasContext.arc(x, y, 2, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = 'white'
    canvasContext.fill()
    canvasContext.lineWidth = 2
    canvasContext.strokeStyle = 'white'
    canvasContext.stroke()
  }

  onContextMenu (event) {}

  equals (presentation) {
    return presentation instanceof Presentation && this.type === presentation.type
  }
}
