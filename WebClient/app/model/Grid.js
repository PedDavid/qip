import { findNearest } from './../util/Math'
import { Point } from './Point'

export default function Grid (initialFigures, _canvasWidth, _canvasHeight, currIdx) {
  let figures = initialFigures    // TODO(simaovii): Change to hashmap
  let canvasWidth = _canvasWidth
  let canvasHeight = _canvasHeight

  this.setCanvasWidth = function (newWidth) {
    canvasWidth = newWidth
  }

  this.setCanvasHeight = function (newHeight) {
    canvasHeight = newHeight
  }

  let currFigureId = currIdx
  // return the new figure idx. If there is already the next idx, return the idx plus 0.1. This is because the concurrency
  this.getNewFigureIdx = function () {
    let toRet = currFigureId + 1
    while (this.hasFigure(toRet)) {
      toRet += 0.1
    }
    currFigureId++
    return toRet
  }

  const GridNode = function (val, height) {
    this.val = val
    this.height = height
  }

  let maxLinePart = 0
  const maxLinePartThreshold = 20

  let grid = [] // dynamic width

  this.export = function () {
    const fig = figures[5]
    fig.points = fig.points.map(point => point.exports())
    console.log(JSON.stringify(fig))
  }

  this.updateMaxLinePart = function (point1, point2, currentFigure, pointStyle) {
    if (point1 === null || point2 === null) { return }
    const distance = Math.sqrt(Math.pow((point1.x - point2.x), 2) + Math.pow((point1.y - point2.y), 2))

    // if distance between points is more than threshold, create some more points and add them to the grid and the figure
    if (distance > maxLinePartThreshold) {
      if (maxLinePart === 0) {
        maxLinePart = maxLinePartThreshold
      }
      const newPoints = this.splitLine(point1, point2, maxLinePart)
        .map(point => this.getOrCreatePoint(point.x, point.y))

      newPoints.forEach(point => point.addFigure(currentFigure.id, pointStyle))
      currentFigure.addPoints(newPoints)
      return
    }

    if (distance > maxLinePart) {
      maxLinePart = Math.round(distance)
    }
  }

  // Get count extra points between two points
  this.splitLine = function (point1, point2, count) {
    count = count + 1

    const d = Math.sqrt((point1.x - point2.x) * (point1.x - point2.x) + (point1.y - point2.y) * (point1.y - point2.y)) / count
    const fi = Math.atan2(point2.y - point1.y, point2.x - point1.x)

    let points = []

    for (let i = 0; i <= count; ++i) {
      points.push(new Point(point1.x + i * d * Math.cos(fi), point1.y + i * d * Math.sin(fi)))
    }

    return points
  }

  this.getMaxLinePart = function () {
    return maxLinePart
  }

  this.addPoints = function (points, figure) {
    points
      .forEach(point => {
        this.getOrCreatePoint(point.x, point.y, point.pointStyle)
          .addFigure(figure.figureId, point.pointStyle)
      })
  }

  // This function adds a point to grid if it does not exists already.
  // Therefore, it needs to know what value in the array is nearest to the value that will be inserted
  this.getOrCreatePoint = function (x, y) {
    const newPoint = new Point(Math.round(x), Math.round(y))
    const newGridNode = new GridNode(x, [newPoint])
    if (grid.length === 0) {
      return insertIntoArray(grid, 0, newGridNode).height[0]
    }

    // check if point.x is already an array's value
    const nearestX = findNearest(grid, x, arrayNode => arrayNode.val)
    if (grid[nearestX].val === x) {
      const nearestY = findNearest(grid[nearestX].height, y, arrayNode => arrayNode.y)
      if (grid[nearestX].height[nearestY].y < y) {
        return insertIntoArray(grid[nearestX].height, nearestY + 1, newPoint)
      } else if (grid[nearestX].height[nearestY].y > y) {
        return insertIntoArray(grid[nearestX].height, nearestY, newPoint)
      }
      return grid[nearestX].height[nearestY] // do nothing if the point already exists
    }
    if (grid[nearestX].val < x) {
      // if the nearest X is lower than X, add to grid in the next index
      return insertIntoArray(grid, nearestX + 1, newGridNode).height[0]
    } else {
      // on the other hand, add in the previous index
      return insertIntoArray(grid, nearestX, newGridNode).height[0]
    }
  }

  function insertIntoArray (array, idxToInsert, toInsert) {
    array.splice(idxToInsert, 0, toInsert)
    return array[idxToInsert]
  }

  this.removePoint = function (x, y) {
    grid[x][y] = null
  }

  this.getFigure = function (figureId) {
    return figures[figureId]
  }

  this.clean = function (context) {
    figures = {}
    currFigureId = 0
    grid = []
    context.clearRect(0, 0, canvasWidth, canvasHeight)
  }

  this.addFigure = function (figure) {
    if (figure.id === null) {
      figure.id = this.getNewFigureIdx()
    }
    let prev = null
    figure.points = figure.points.map(point => {
      const gridPoint = this.getOrCreatePoint(point.x, point.y)
      gridPoint.addFigure(figure.id, point.style)
      this.updateMaxLinePart(prev, gridPoint, figure.id, point.style)
      prev = gridPoint
      return gridPoint
    })
    figures[figure.id] = figure
  }

  this.removeFigureFromGrid = function (figureId) {
    delete figures[figureId]
  }

  this.removeFigure = function (figure, context, currScale) {
    this.removeFigureFromGrid(figure.id)
    this.draw(context, currScale)
    // remove figure from all points
    figure.points.forEach(point => point.removeFigure(figure.id))
  }

  this.draw = function (context, currScale) {
    context.clearRect(0, 0, canvasWidth, canvasHeight)

    Object.entries(figures) // devolve um array (length = soma de todas as propriedades do objeto) de arrays de [key, value].
                            // Cada array [key, value] tem a chave do atributo assim como o valor guardado nesse atributo
                            // nota: não usar localecompare. não tem o efeito desejado
        .sort(([K1], [K2]) => K1 - K2) // são passados à função sort o 1º e 2º índices do array, que por sua vez, são arrays. Ao usar destructors, apenas são obtidos as chaves de cada array
        .forEach(([id, f]) => f.draw(context, currScale))
  }

  this.getWidth = function () {
    // return grid.length;
    return canvasWidth
  }

  this.getHeight = function () {  // TODO(simaovii): ver se vale a pena receber a coluna como parametro
    // return grid[0].length;
    return canvasHeight
  }

  this.getFigures = function () {
    return figures
  }

  this.hasFigure = function (figureId) {
    return figures[figureId] != null
  }

  this.getNearestFigures = function (pointX, pointY) {
    const x = Math.round(pointX)
    const y = Math.round(pointY)
    let figuresToRet = new Set()
    // BUG(peddavid): throws error if grid is empty
    const minX = findNearest(grid, x - maxLinePart, arrayNode => arrayNode.val)
    if (grid[minX].val < x - maxLinePart * 2) { // margin
      return []
    }
    const maxX = findNearest(grid, x + maxLinePart, arrayNode => arrayNode.val)
    for (let widthNode = grid[minX], widthIdx = minX; widthIdx < grid.length && widthIdx <= maxX; widthNode = grid[++widthIdx]) {
      const minY = findNearest(widthNode.height, y - maxLinePart, arrayNode => arrayNode.y)
      if (widthNode.height[minY].y < y - maxLinePart * 2) {
        continue
      }
      const maxY = findNearest(widthNode.height, y + maxLinePart, arrayNode => arrayNode.y)
      for (let heightNode = widthNode.height[minY], heightIdx = minY; heightIdx < widthNode.height.length && heightIdx <= maxY; heightNode = widthNode.height[++heightIdx]) {
        heightNode.getFigureIds().forEach(pointFigure => figuresToRet.add(pointFigure))
      }
    }
    return Array.from(figuresToRet.values()).map(figureId => figures[figureId])
  }
}
