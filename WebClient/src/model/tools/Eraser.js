import { Point } from './../Point'
import { Figure } from './../Figure'
import { Rect } from './../Rect'

export function CanvasEraser (grid, canvasContext, items) {
  this.erasing = false
  let movingEraser
  let eraseLine
  let currentEraserSize

  this.onDown = function (currentItem, coord, event, currScale) {
    // event.clientX - 5 + document.body.scrollLeft, event.clientY - 5 + document.body.scrollTop
    this.erasing = true
    currentEraserSize = items.erasers.filter(eraser => eraser.name === currentItem.eraser.name)[0].size

    erase(coord.x, coord.y, currScale)

    const point = new Point(Math.round(coord.x), Math.round(coord.y))
    eraseLine = new Figure('eraseLineId')
    eraseLine.addPoint(point)

    // if (event.button == 5) {
    // this.erasedByStylus = true
    // changeStyle(document.getElementById("eraser"))
  }

  this.onMove = function (currItem, coord, event, currScale) {
    // verificar se o evento ocorreu nas mesmas coordenadas que anteriormente
    if ((eraseLine == null || eraseLine.points[0].x === coord.x) && eraseLine.points[0].y === coord.y) {
      return // nesse caso ignorá-lo
    }

    const point = new Point(Math.round(coord.x), Math.round(coord.y))
    eraseLine.addPoint(point)
    movingEraser = true
    erase(coord.x, coord.y, currScale)

    // After erase, reset eraseLine. This way eraseLine always has 2 points, optimizing line iteration in method erase()
    eraseLine = new Figure('eraseLineId')
    eraseLine.addPoint(point)

    // this.erase(coord.x, coord.y, event)
    // context.clearRect(event.clientX - 5 + document.body.scrollLeft, event.clientY - 5 + document.body.scrollTop, 15, 15) //verificar se está a apagar (não é um algoritmo bonito)
  }

  this.onUp = function () {
    this.onOut()
  }

  this.onOut = function () {
    this.erasing = false
    if (movingEraser) {
      movingEraser = false
    }
    eraseLine = null
  }

  const erase = function (x, y, currScale) {
    // calcular limites da area a apagar
    const lix = x / currScale - currentEraserSize / 2
    const liy = y / currScale - currentEraserSize / 2
    const lsx = x / currScale + currentEraserSize / 2
    const lsy = y / currScale + currentEraserSize / 2

    // const figures = grid.getFigures()
    // obter apenas as figuras que estão próximas da borracha
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
        const width = currPoint.getStyleOf(figure.id).width
        const rect = new Rect(prev, currPoint, width, canvasContext)
        // const rect = myContext.getRect(prev, currPoint, width)
        if (movingEraser) {
          // TODO(simaovii): não verifica se toda a área da borracha interseta a linha desenhada
          if (intersectsLine(eraseLine, prev, currPoint)) {
            grid.removeFigure(figure, canvasContext, currScale)
            return
          }
        } else {
          // TODO(simaovii): ver se nao basta iterar sobre os 4 pontos maximos da borracha
          for (let c = lix > 0 ? lix : 0; c < grid.getWidth() && c < lsx; ++c) {
            for (let l = liy > 0 ? liy : 0; l < grid.getHeight() && l < lsy; ++l) {
              if (rect.contains({ x: c, y: l })) {
                grid.removeFigure(figure, canvasContext, currScale)
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
  const intersectsLine = function (eraseLine, lineP1, lineP2) {
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
    let figLineM = figLineA / figLineB
    const figLineC = auxLineP2.x * lineP1.y - lineP1.x * auxLineP2.y
    let figLineN = figLineC / figLineB

    const points = eraseLine.points

    var prev = points[0]
    const currPoint = points[1]

    let pointBelongsToFirstLine = false
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
    if (intersectX <= auxLineP2.x && intersectX >= auxLineP1.x && intersectY <= auxLineP2.y && intersectY >= auxLineP1.y) {
      pointBelongsToFirstLine = true
    } else {
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
      if (pointBelongsToFirstLine) {
        return true
      }
    }
    return false
  }
}
