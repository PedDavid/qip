import { PointStyle } from './../Point'
import { Figure, FigureStyle } from './../Figure'

export function Pen (grid, canvasContext, onUpdateCanvasSize) {
  this.currentFigure = null
  // passar para aqui as funções getMousePos
  // passar como param updateCanvasSize??
  this.onDown = function (currentItem, coord, event, scale) {
    const figStyle = new FigureStyle(currentItem.pen.name, scale)
    // Create a new Figure
    this.currentFigure = new Figure(grid.getNewFigureIdx(), null, figStyle)

    let press = event.pressure * currentItem.pen.width
    const pointStyle = new PointStyle(press)
    // Get the point
    const point = grid.getOrCreatePoint(coord.x, coord.y)
    // Add point to the Figure
    this.currentFigure.addPoint(point)
    // Add Figure to the point
    point.addFigure(this.currentFigure.id, pointStyle)

    canvasContext.beginPath()
    canvasContext.arc(coord.x, coord.y, press / 4, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = currentItem.pen.name
    canvasContext.fill()
    canvasContext.lineWidth = press / 2
    canvasContext.strokeStyle = currentItem.pen.name
    canvasContext.stroke()
    onUpdateCanvasSize(coord.x, coord.y)
  }

  this.onMove = function (currentItem, coord, event, scale) {
    if (event.buttons > 0) {
      if (this.currentFigure == null) {
        return
      }

      let press = event.pressure * currentItem.pen.width
      const pointStyle = new PointStyle(press)

      canvasContext.beginPath() // também tem de estar aqui para dar para fazer pressure sensitive

      const point = grid.getOrCreatePoint(coord.x, coord.y)

      // esta situação pode acontecer quando o utilizador sai fora dos limites do canvas
      // caso aconteça, acrescentar o primeiro ponto
      if (this.currentFigure.points.length === 0) {
        point.addFigure(this.currentFigure.id, pointStyle)
        this.currentFigure.addPoint(point)
        return
      }

      const last = this.currentFigure.points[this.currentFigure.points.length - 1]

      if (last.x === point.x && last.y === point.y) {   // verificar se o evento ocorreu nas mesmas coordenadas que anteriormente
        return                                        // nesse caso ignorá-lo
      }

      point.addFigure(this.currentFigure.id, pointStyle)
      this.currentFigure.addPoint(point)

      grid.updateMaxLinePart(last, point, this.currentFigure, pointStyle) // atualizar a variável que guarda o valor máximo entre pontos intermédios de retas
      const width = event.pressure * currentItem.pen.width

      canvasContext.moveTo(last.x, last.y)
      canvasContext.lineTo(point.x, point.y)

      canvasContext.lineWidth = width
      canvasContext.lineJoin = canvasContext.lineCap = 'round'
      canvasContext.stroke()

      onUpdateCanvasSize(coord.x, coord.y)
    }
  }

  this.onUp = function () {
    grid.addFigure(this.currentFigure)
    this.currentFigure = null
  }

  this.onOut = function (currentItem) {
    if (this.currentFigure == null) {
      return
    }
    grid.addFigure(this.currentFigure)

    // fazer reset à figura para que não continue a desenhar se o utilizador sair da área do canvas
    // Desta forma, se o utilizador voltar à área do canvas com o ponteiro premido, irá desenhar uma nova figura
    const figStyle = new FigureStyle(currentItem.pen.name)
    this.currentFigure = new Figure(grid.getNewFigureIdx(), null, figStyle)
  }
}
