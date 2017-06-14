import { Figure, FigureStyle } from './../Figure'

import Tool from './Tool'

export default class Pen implements Tool {
  constructor (grid, color, width) {
    this.color = color
    this.width = width

    this.grid = grid
    this.currentFigure = null
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale) {
    const x = event.offsetX
    const y = event.offsetY

    const figStyle = new FigureStyle(this.color, scale)
    // Create a new Figure
    this.currentFigure = new Figure(figStyle)

    const press = event.pressure * this.width
    this.currentFigure.addPoint({x, y, style: { press }})

    const canvasContext = event.target.getContext('2d')
    canvasContext.beginPath()
    canvasContext.arc(x, y, press / 4, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = this.color
    canvasContext.fill()
    canvasContext.lineWidth = press / 2
    canvasContext.strokeStyle = this.color
    canvasContext.stroke()
  }

  onSwipe (event, scale) {
    if (event.buttons > 0) {
      const x = event.offsetX
      const y = event.offsetY

      if (this.currentFigure === null) {
        return
      }

      const press = event.pressure * this.width
      const last = this.currentFigure.points[this.currentFigure.points.length - 1]
      // Ignore if swiped to last coordinates
      if (last.x === x && last.y === y) {
        return
      }
      this.currentFigure.addPoint({x, y, style: { press }})

      const canvasContext = event.target.getContext('2d')
      canvasContext.beginPath() // também tem de estar aqui para dar para fazer pressure sensitive

      canvasContext.moveTo(last.x, last.y)
      canvasContext.lineTo(x, y)

      canvasContext.lineWidth = press
      canvasContext.lineJoin = canvasContext.lineCap = 'round'
      canvasContext.stroke()
    }
  }

  onPressUp () {
    this.grid.addFigure(this.currentFigure)
    this.currentFigure = null
  }

  onOut () {
    const grid = this.grid
    if (this.currentFigure === null) {
      return
    }
    grid.addFigure(this.currentFigure)

    // fazer reset à figura para que não continue a desenhar se o utilizador sair da área do canvas
    // Desta forma, se o utilizador voltar à área do canvas com o ponteiro premido, irá desenhar uma nova figura
    const figStyle = new FigureStyle(this.color)
    this.currentFigure = new Figure(figStyle)
  }
}
