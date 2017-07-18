import { findNearest } from './../util/Math'
import { Point, PointStyle } from './Point'
import { Figure, FigureStyle } from './Figure'
import { SimplePoint } from './SimplePoint'

export default function Grid (initialFigures, currIdx) {
  let figures = new Map()    // TODO(simaovii): Change to hashmap
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

  this.getCurrentFigureId = function () {
    return currFigureId
  }

  this.updateCurrentFigIdIfGreater = function (newMaxId) {
    if (currFigureId < newMaxId) {
      currFigureId = newMaxId
    }
  }

  const GridNode = function (val, height) {
    this.val = val
    this.height = height
  }

  let maxLinePart = 0
  const maxLinePartThreshold = 20

  let grid = [] // dynamic width

  this.updateMaxLinePart = function (point1, point2, currentFigure, pointStyle) {
    if (point1 === null || point2 === null) { return }
    const distance = Math.sqrt(Math.pow((point1.x - point2.x), 2) + Math.pow((point1.y - point2.y), 2))

    // if distance between points is more than threshold, create some more points and add them to the grid and the figure
    if (distance > maxLinePartThreshold) {
      if (maxLinePart === 0) {
        maxLinePart = maxLinePartThreshold
      }
      let newPoints = this.splitLine(point1, point2, maxLinePart)
      newPoints = newPoints.map(point => this.getOrCreatePoint(point.x, point.y))

      newPoints.forEach(point => {
        point.addFigure(currentFigure.id, pointStyle)
        currentFigure.addPoint(point)
      })
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
    return figures.get(figureId)
  }

  this.clean = function (context) {
    const { width, height } = context.canvas
    figures = new Map()
    currFigureId = 0
    grid = []
    context.clearRect(0, 0, width, height)
  }

  this.addFigure = function (figure) {
    if (figure.id === null) {
      figure.id = this.getNewFigureIdx()
    }
    let prev = null
    const auxPoints = figure.points
    figure.points = [] // must be that way because in forEach updateMaxLinePart is also adding points to figure
    // TODO(peddavid): FlatMap then? Instead of this auxPoints and mutation in points?
    auxPoints.forEach(simplePoint => {
      const gridPoint = this.getOrCreatePoint(simplePoint.x, simplePoint.y)
      // gridPoint.addFigure(figure.id, point.style)
      figure.addPoint(gridPoint)
      const pointStyle = new PointStyle(simplePoint.style.width) // actualy, SimplePoint.style is a PointStyle but as the data is being parsed from json, it must be instantiated again
      gridPoint.addFigure(figure.id, pointStyle)
      this.updateMaxLinePart(prev, gridPoint, figure, pointStyle)
      prev = gridPoint
    })
    figures.set(figure.id, figure)
  }

  this.removeFigureFromGrid = function (figureId) {
    figures.delete(figureId)
  }

  this.removeFigure = function (figure, context, currScale) {
    this.removeFigureFromGrid(figure.id)
    this.draw(context, currScale)
    // remove figure from all points
    figure.points.forEach(point => point.removeFigure(figure.id))
  }

  this.moveImage = function (img, newSrcPoint, context, currScale) {
    img.setSrcPoint(newSrcPoint)
    this.draw(context, currScale)
  }

  this.moveFigure = function (figure, destPoint, context, scale) {
    const destPointOffsetX = destPoint.x
    const destPointOffsetY = destPoint.y
    // criar todos os novos pontos novamente, com as novas posições
    let pointsClone = figure.points.map(point => {
      return new SimplePoint(point.x, point.y, point.getStyleOf(figure.id))
    })

    // nota: não tentar otimizar sem ver os problemas - é necessário estas variáveis e ordem de operações
    // é necessário este forEach adicional pois as figuras são removidas dos pontos anteriores. Estava a haver conflitos com os pontos criados, que estavam a remover a figura do ponto anterior (mas esse ponto já estava a ser usado na nova figura)
    figure.points.forEach(point => point.removeFigure(figure.id)) // remover figura de todos os seus pontos

    pointsClone = pointsClone.map(point => {
      const newPoint = this.getOrCreatePoint(point.x + destPointOffsetX, point.y + destPointOffsetY)
      newPoint.addFigure(figure.id, point.style)
      return newPoint
    })

    figure.points = pointsClone // reset points from figure
    this.draw(context, scale)
  }

  this.draw = function (context, currScale) {
    const { width, height } = context.canvas
    context.clearRect(0, 0, width, height)

    Array.from(figures.values())
        .sort((fig1, fig2) => fig1.id - fig2.id)
        .forEach((figure) => figure.draw(context, currScale))
  }

  this.getFigures = function () {
    return Array.from(figures.values())
  }

  this.getImageFigures = function () {
    /* Object.entries(figures)
      .map(([figId, fig]) => fig)
      .filter(fig => fig instanceof FigureImage) */
  }

  this.hasFigure = function (figureId) {
    return figures.has(figureId)
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
      for (let heightNode = widthNode.height[minY], heightIdx = minY;
              heightIdx < widthNode.height.length && heightIdx <= maxY;
              heightNode = widthNode.height[++heightIdx]) {
        heightNode.getFigureIds().forEach(pointFigure => figuresToRet.add(pointFigure))
      }
    }
    return Array.from(figuresToRet.values()).map(figureId => figures.get(figureId))
  }

  // map initial figures to Figure Objects and add them to figure array
  initialFigures.forEach(initFig => {
    const figStyle = new FigureStyle(initFig.style.color, initFig.style.scale)
    const newFigure = new Figure(figStyle, initFig.tempId)
    newFigure.points = initFig.points
    this.addFigure(newFigure)
  })
}
