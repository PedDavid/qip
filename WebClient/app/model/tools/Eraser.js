import { Point } from './../Point'
import Line from './../Line'
import { SimplePoint } from './../SimplePoint'
import { Rect } from './../Rect'

import Tool from './Tool'
import {Figure} from './../Figure'

export default class Eraser implements Tool {
  constructor (grid, width, eraserType) {
    this.type = 'eraser'
    this.width = width
    this.fontText = eraserType === 'normal' ? width : 'S' // todo: this should be specified in defaultTools
    this.grid = grid
    this.eraseLine = null
    this.erasedFigures = []
    this.addedFigures = []
    this.eraserType = eraserType
  }

  onPress (event, currScale) {
    const coord = { x: event.offsetX, y: event.offsetY }

    this.erase(coord.x, coord.y, currScale, event.target)

    const point = new Point(coord.x, coord.y)
    this.eraseLine = new Line(null, point)
  }

  onSwipe (event, currScale) {
    if (event.buttons <= 0) {
      return
    }

    const coord = { x: event.offsetX, y: event.offsetY }
    const previousPoint = this.eraseLine.end
    // Ignore if swiped to last coordinates
    if (previousPoint === null || (previousPoint.x === coord.x && previousPoint.y === coord.y)) {
      return
    }

    const point = new Point(coord.x, coord.y)
    this.eraseLine = new Line(previousPoint, point)
    this.erase(coord.x, coord.y, currScale, event.target)
  }

  onPressUp (event, persist) {
    this.onOut(event, persist)
  }

  onOut (event, persist) {
    const canvas = event.target
    const canvasContext = canvas.getContext('2d')

    this.erasedFigures.forEach(fig => {
      this.grid.removeFigure(fig, canvasContext, 1, true)
      persist.sendEraserAction(fig.id)
    })

    this.addedFigures.forEach((fig, idx) => {
      this.grid.addFigure(fig, true)
      persist.sendPenAction(fig, this.grid.getCurrentFigureId())
    })

    if (this.addedFigures.length > 0) {
      this.grid.draw(canvasContext, 1)
    }

    this.erasedFigures = []
    this.addedFigures = []
    this.eraseLine = null // TODO(simaovii): this will throw error if user draws back in the canvas. insert a check in onSwipe
  }

  onContextMenu (event) {}

  equals (eraser) {
    return eraser instanceof Eraser && this.width === eraser.width && this.eraserType === eraser.eraserType
  }

  erase (x, y, currScale, canvas) {
    const grid = this.grid
    // calcular limites da area a apagar
    const lix = x / currScale - this.width / 2
    const liy = y / currScale - this.width / 2
    const lsx = x / currScale + this.width / 2
    const lsy = y / currScale + this.width / 2

    const figures = grid
        .getNearestFigures(x / currScale, y / currScale)
        .sort((fig1, fig2) => fig2.id - fig1.id) // inverse order to remove the newest first

    for (const prop in figures) {
      const figure = figures[prop]
      const points = figure.getNearPoints(x / currScale, y / currScale, grid.getMaxLinePart())
      if (figure.points == null) {
        return
      }

      let prev = points[0]
      for (let k = 0; k <= points.length; k++) {
        let currPoint = points[k]
        if (currPoint == null) {
          break
        }
        prev.x = prev.x * currScale
        prev.y = prev.y * currScale
        currPoint.x = currPoint.x * currScale
        currPoint.y = currPoint.y * currScale
        // TODO(peddavid, simaovii): width shadowed?
        const width = currPoint.getStyleOf(figure.id).width
        const canvasContext = canvas.getContext('2d')
        const rect = new Rect(prev, currPoint, width, canvasContext)
        const canvasWidth = canvasContext.canvas.width
        const canvasHeight = canvasContext.canvas.height
        if (this.eraseLine !== null) {
          // TODO(simaovii): não verifica se toda a área da borracha interseta a linha desenhada
          if (this.intersectsLine(this.eraseLine, prev, currPoint)) {
            if (this.eraserType === 'normal') {
              // const figStyle = figure.figureStyle
              // const figurePoints = figure.getSimplePoints()
              // const pointsToErase = figure.getNearPoints(currPoint.x, currPoint.y, this.width)
              // // erase all figure
              // grid.removeFigure(figure, canvasContext, currScale)
              // this.erasedFigures.push(figure)
              // // create new figures from the remaining points

              // let currentFigure = new Figure(figStyle)
              // this.addedFigures = [currentFigure]
              // let currIdx = -1
              // for (let point in figurePoints){
              //   currIdx++
              //   if (points.idx > currIdx + 1){
              //     currIdx = points.idx - 1
              //     this.addedFigures.push(currentFigure)
              //     currentFigure = new Figure(figStyle)
              //   }
              //   currentFigure.addPoint(point)
              // }
            } else if (this.eraserType === 'stroke') {
              grid.removeFigure(figure, canvasContext, currScale, true)
              this.erasedFigures.push(figure)
            }
            return
          }
        } else {
          // TODO(simaovii): ver se nao basta iterar sobre os 4 pontos maximos da borracha
          for (let c = lix > 0 ? lix : 0; c < canvasWidth && c < lsx; ++c) {
            for (let l = liy > 0 ? liy : 0; l < canvasHeight && l < lsy; ++l) {
              if (rect.contains({ x: c, y: l })) {
                if (this.eraserType === 'normal') {
                  const figStyle = figure.figureStyle
                  const figurePoints = figure.getSimplePoints()
                  // const pointsToErase = figure.getNearPoints(currPoint.x, currPoint.y, this.width * 2)

                  const rect = new Rect(new SimplePoint(x - this.width, y), new SimplePoint(x + this.width, y), this.width * 2, canvasContext)
                  const pointsToErase = figurePoints.filter(point => {
                    return rect.contains(point)
                  })
                  const figuresToAdd = []
                  let currentFig = new Figure(figStyle)
                  for (let pointIdx in figurePoints) {
                    const point = figurePoints[pointIdx]
                    // check if current point was erased
                    if (pointsToErase.find(pointToErase => pointToErase.x === point.x && pointToErase.y === point.y) !== undefined) {
                      if (currentFig.points.length <= 0) {
                        continue
                      }
                      if (currentFig.points.length > 1) { // dispensa linhas com apenas um ponto
                        figuresToAdd.push(currentFig)
                      }
                      currentFig = new Figure(figStyle)
                      continue
                    }
                    currentFig.addPoint(point)
                  }
                  if (currentFig.points.length > 1) {
                    figuresToAdd.push(currentFig)
                  }

                  this.addedFigures = figuresToAdd

                  // erase all figure
                  this.erasedFigures.push(figure)

                  console.log('numero de figuras a inserir: ' + this.addedFigures.length)
                } else if (this.eraserType === 'stroke') {
                  this.erasedFigures.push(figure)
                }
                return
              }
            }
          }
        }
        prev = currPoint
      }
    }
  }

  // TODO(simaovii): por esta função em Figure para não ter que passar eraseLine
  intersectsLine (eraseLine, lineP1, lineP2) {
    // Por agora, este método apenas verifica se a linha da borracha intersecta a linha da figura.
    // É de ter em conta que esta linha não conta com a espessura. Se for necessário tem de se alterar este método para verificar se a linha
    // da borracha intersecta alguma linha do rectângulo

    // calculate figure line equation => y=mx+n
    let auxLineP2 = { x: lineP2.x, y: lineP2.y }
    const figLineA = auxLineP2.y - lineP1.y
    let figLineB = auxLineP2.x - lineP1.x
    // TODO(cipri): não te esqueças de otimizar, quando as linhas são verticais
    // if line is vertical, we assume that it is not. This deviation do not change the behavior of the algorithm as the line segment distance is too short
    if (figLineB === 0) {
      auxLineP2.x = auxLineP2.x - 1
      figLineB = auxLineP2.x - lineP1.x
    }
    const figLineM = figLineA / figLineB
    const figLineC = auxLineP2.x * lineP1.y - lineP1.x * auxLineP2.y
    const figLineN = figLineC / figLineB

    const prev = eraseLine.start
    const currPoint = eraseLine.end

    // TODO(simaovii): guardar em cache a equação da reta da borracha pois vai ser a mesma para os restantes pontos da figura assim como para as restantes figuras do canvas
    // calculate part of erase line equation => y=mx+n
    const eraseLineA = currPoint.y - prev.y
    let eraseLineB = currPoint.x - prev.x
    // if line is vertical, we assume that it is not. This deviation do not change the behaviour of the algorithm as the line segment distance is too short
    if (eraseLineB === 0) {
      currPoint.x = currPoint.x - 1
      eraseLineB = currPoint.x - prev.x
    }
    // the to lines are vertical ou horizontal -> do not intersect each other
    if ((eraseLineB === 0 && figLineB === 0) || (eraseLineA === 0 && figLineA === 0)) {
      return false
    }

    let eraseLineM = eraseLineA / eraseLineB

    // the two lines are parallel
    if (eraseLineM === figLineM) {
      return false
    }

    const eraseLineC = currPoint.x * prev.y - prev.x * currPoint.y
    let eraseLineN = eraseLineC / eraseLineB

    // calculate point of intersection of the to lines
    const intersectY = ((figLineM * eraseLineN) - (eraseLineM * figLineN)) / (figLineM - eraseLineM)
    const intersectX = (eraseLineN - figLineN) / (figLineM - eraseLineM)

    // check if intersection point of the two lines belongs to both segments
    // segment 1
    // ordenar os pontos. é necessário atribuir os novos pontos a novas variáveis para não mexer com os dados das linhas
    const auxLineP1 = { x: lineP1.x, y: lineP1.y }

    if (auxLineP2.x < lineP1.x) { auxLineP1.x = auxLineP2.x; auxLineP2.x = lineP1.x }
    if (auxLineP2.y < lineP1.y) { auxLineP1.y = auxLineP2.y; auxLineP2.y = lineP1.y }

    // check if intersection point is inside segment interval of figure line
    if (!(intersectX <= auxLineP2.x && intersectX >= auxLineP1.x && intersectY <= auxLineP2.y && intersectY >= auxLineP1.y)) {
      return false
    }

    // check if intersection point of the two lines belongs to both segments
    // segment 2
    // ordenar os pontos. é necessário atribuir os novos pontos a novas variáveis para não mexer com os dados das linhas
    const auxCurrPoint = { x: currPoint.x, y: currPoint.y }
    const auxprevPoint = { x: prev.x, y: prev.y }
    if (currPoint.x < prev.x) { auxCurrPoint.x = prev.x; auxprevPoint.x = currPoint.x }
    if (currPoint.y < prev.y) { auxCurrPoint.y = prev.y; auxprevPoint.y = currPoint.y }

    // check if intersection point is inside segment interval of erase line
    if (intersectX <= auxCurrPoint.x && intersectX >= auxprevPoint.x && intersectY <= auxCurrPoint.y && intersectY >= auxprevPoint.y) {
      return true
    }
    return false
  }
}
