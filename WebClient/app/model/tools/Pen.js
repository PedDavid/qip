import { PointStyle } from './../Point'
import { Figure, FigureStyle } from './../Figure'

import Tool from './Tool'

export default class Pen implements Tool {
  constructor (grid, name, width) {
    this.name = name
    this.width = width

    this.grid = grid
    this.currentFigure = null
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale) {
    const x = event.offsetX
    const y = event.offsetY

    const figStyle = new FigureStyle(this.name, scale)
    // Create a new Figure
    this.currentFigure = new Figure(this.grid.getNewFigureIdx(), null, figStyle)

    let press = event.pressure * this.width
    const pointStyle = new PointStyle(press)
    // Get the point
    const point = this.grid.getOrCreatePoint(x, y)
    // Add point to the Figure
    this.currentFigure.addPoint(point)
    // Add Figure to the point
    point.addFigure(this.currentFigure.id, pointStyle)

    const canvasContext = event.target.getContext('2d')
    canvasContext.beginPath()
    canvasContext.arc(x, y, press / 4, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = this.name
    canvasContext.fill()
    canvasContext.lineWidth = press / 2
    canvasContext.strokeStyle = this.name
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
      const pointStyle = new PointStyle(press)

      const canvasContext = event.target.getContext('2d')
      canvasContext.beginPath() // também tem de estar aqui para dar para fazer pressure sensitive

      const point = this.grid.getOrCreatePoint(x, y)

      // esta situação pode acontecer quando o utilizador sai fora dos limites do canvas
      // caso aconteça, acrescentar o primeiro ponto
      if (this.currentFigure.points.length === 0) {
        point.addFigure(this.currentFigure.id, pointStyle)
        this.currentFigure.addPoint(point)
        return
      }

      const last = this.currentFigure.points[this.currentFigure.points.length - 1]
      // Ignore if swiped to last coordinates
      if (last.x === point.x && last.y === point.y) {
        return
      }
      point.addFigure(this.currentFigure.id, pointStyle)
      this.currentFigure.addPoint(point)

      this.grid.updateMaxLinePart(last, point, this.currentFigure, pointStyle) // atualizar a variável que guarda o valor máximo entre pontos intermédios de retas
      const width = event.pressure * this.width

      canvasContext.moveTo(last.x, last.y)
      canvasContext.lineTo(point.x, point.y)

      canvasContext.lineWidth = width
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
    const figStyle = new FigureStyle(this.name)
    this.currentFigure = new Figure(grid.getNewFigureIdx(), null, figStyle)
  }
}
