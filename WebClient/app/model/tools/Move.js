import { Point } from './../Point'
import { Rect } from './../Rect'
import { Figure } from './../Figure'
import Line from './../Line'
// import { FigureImage } from './../FigureImage'

import Tool from './Tool'

export default class Move implements Tool {
  constructor (grid) {
    this.grid = grid
    this.movingLine = null
    this.currentFigureMoving = null
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale) { // current
    const x = event.offsetX
    const y = event.offsetY

    // get line figures
    const lines = this.grid
      .getNearestFigures(x / scale, y / scale)
      .filter(lin => this.figureContainsPoint(lin, {x, y}, event, scale)) // todo: change {x,y} to simple point obj

    // get image figures
    /*
    const images = this.grid.getImageFigures()
      .filter(img => this.imgContainsPoint(img, {x,y}, event))
    */
    // juntar linhas e imagens
    // ordenar linhas e imagens
    const figures = lines// .concat(images)
      .sort((fig1, fig2) => fig2.id - fig1.id) // inverse order to remove the newest first

    this.currentFigureMoving = figures[0]
    const point = new Point(x, y)
    this.movingLine = new Line(point, point)
  }

  onSwipe (event, scale) {
    if (event.buttons <= 0) {
      return
    }
    const x = event.offsetX
    const y = event.offsetY
    const canvasContext = event.target.getContext('2d')

    /*
    if (this.movingLine!=null && this.currentFigureMoving instanceof FigureImage) {
      const srcPoint = this.currentFigureMoving.getSrcPoint()
      const destPoint = new Point(
        srcPoint + (x - this.movingLine.end.x),
        srcPoint + (y - this.movingLine.end.y))
      this.grid.moveImage(this.currentFigureMoving, destPoint, canvasContext, scale)
    }
    else
    */
    if (this.movingLine != null && this.currentFigureMoving instanceof Figure) {
      const offsetPoint = new Point(x - this.movingLine.end.x, y - this.movingLine.end.y)
      this.grid.moveFigure(this.currentFigureMoving, offsetPoint, canvasContext, scale)
    }
    this.movingLine = new Line(this.movingLine.start, {x, y}) // always preserve start point and update final point
  }

  onPressUp (event) {
    this.onOut(event)
  }

  onOut (event) {
    const canvasContext = event.target.getContext('2d')
    const scale = 1  // TODO(simaovii): eliminate this when using sockets
    // if (this.lastPoint!=null && this.currentFigureMoving instanceof FigureImage) {
      // send by websockets currentFigureMoving(id), destPoint, scale?
    // }
    if (this.movingLine != null && this.currentFigureMoving instanceof Figure) {
      const offsetPoint = new Point(this.movingLine.end.x - this.movingLine.start.x, this.movingLine.end.y - this.movingLine.start.y)
      this.grid.moveFigure(this.currentFigureMoving, offsetPoint, canvasContext, scale) // TODO(simaovii): eliminate this when using sockets
      // send by websockets currentFigureMoving(id), offsetPoint, scale
    }
    this.movingLine = null
    this.currentFigureMoving = null
  }

  // todo: this function should be in Image Class
  imgContainsPoint = function (img, point, event) {
    const canvasContext = event.target.getContext('2d')
    const imgHeight = img.getHeight()
    const initPoint = new Point(img.getSrcPoint().x, img.getSrcPoint().y + imgHeight / 2)
    const endPoint = new Point(img.getSrcPoint().x + img.getWidth(), img.getSrcPoint().y + imgHeight / 2)

    const rect = new Rect(initPoint, endPoint, imgHeight, canvasContext)

    return rect.contains(new Point(point.x, point.y))
  }

  // todo: this function should be in Figure Class
  figureContainsPoint = function (figure, coord, event, scale) {
    const canvasContext = event.target.getContext('2d')
    const points = figure.getNearPoints(coord.x / scale, coord.y / scale, this.grid.getMaxLinePart())

    if (figure.points == null) {
      return
    }

    let prev = points[0]
    for (let k = 0; k <= points.length; k++) {
      let currPoint = points[k]
      if (currPoint == null) {
        break
      }

      prev.x = prev.x * scale
      prev.y = prev.y * scale
      currPoint.x = currPoint.x * scale
      currPoint.y = currPoint.y * scale
      const width = currPoint.getStyleOf(figure.id).width
      const rect = new Rect(prev, currPoint, width, canvasContext)

      const canvasWidth = canvasContext.canvas.width
      const canvasHeight = canvasContext.canvas.height

      // check if coord is inside line, adding some margin
      for (let c = (coord.x - 10) > 0 ? (coord.x - 10) : 0; c < canvasWidth && c < coord.x + 10; ++c) {
        for (let l = (coord.y - 10) > 0 ? (coord.y - 10) : 0; l < canvasHeight && l < (coord.y + 10); ++l) {
          if (rect.contains({ x: c, y: l })) {
            return true
          }
        }
      }
      prev = currPoint
    }
    return false
  }
}