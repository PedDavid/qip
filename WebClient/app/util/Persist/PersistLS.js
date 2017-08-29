import Grid from './../../model/Grid'
import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Move from './../../model/tools/Move'
import BoardData from './../../model/BoardData'

export default class PersistLS {
  // get initial board from local storage
  static _getInitialBoardLS () {
    console.info('getting board data from local storage')
    return new Promise((resolve, reject) => {
      // if it's not authenticated or not sharing board, get data from localstorage
      if (window.localStorage.getItem('figures') === null || window.localStorage.getItem('canvasSize') === null) {
        this._resetLocalStorage()
      }

      const figures = JSON.parse(window.localStorage.getItem('figures'))
      const nextFigureId = JSON.parse(window.localStorage.getItem('currFigureId'))
      const grid = new Grid(figures, nextFigureId)
      const canvasSize = JSON.parse(window.localStorage.getItem('canvasSize'))
      const initBoard = {
        grid,
        canvasSize
      }

      resolve(initBoard)
    })
  }

  static _cleanCanvasLS = function () {
    this._resetLocalStorage()
  }

  static _updateFavoritesLS = function (newFavs) {
    window.localStorage.setItem('favorites', JSON.stringify(newFavs))
  }

  static _updateCurrToolLS = function (newCurrTool) {
    window.localStorage.setItem('currTool', JSON.stringify(newCurrTool))
  }

  static _updateCanvasSizeLS = function (canvasSize) {
    window.localStorage.setItem('canvasSize', JSON.stringify(canvasSize))
  }

  // get user info from local storage
  static _getUserInfoAsyncLS = function (grid) {
    console.info('getting user info from local storage')
    return new Promise((resolve, reject) => {
      // if it's not authenticated or not sharing board, get data from localstorage
      if (window.localStorage.getItem('defaultPen') === null || window.localStorage.getItem('defaultEraser') === null ||
        window.localStorage.getItem('currTool') === null || window.localStorage.getItem('favorites') === null ||
        window.localStorage.getItem('settings') === null) {
        this._resetLocalStorage()
      }

      const tempPen = JSON.parse(window.localStorage.getItem('defaultPen'))
      const tempEraser = JSON.parse(window.localStorage.getItem('defaultEraser'))
      const tempCurrTool = JSON.parse(window.localStorage.getItem('currTool'))
      const favorites = JSON.parse(window.localStorage.getItem('favorites'))
        .map(fav => this._getToolFromLS(grid, fav))
      const settings = JSON.parse(window.localStorage.getItem('settings'))
      const userInfo = {
        defaultPen: new Pen(grid, tempPen.color, tempPen.width),
        defaultEraser: new Eraser(grid, tempEraser.width),
        currTool: this._getToolFromLS(grid, tempCurrTool),
        favorites,
        settings
      }

      resolve(userInfo)
    })
  }

  // get board info from local storage
  static _getBoardInfoLS = function (boardId) {
    console.info('getting board info from local storage')
    return new Promise((resolve, reject) => {
      // users not authenticated are not able to store boards.
      // therefore, when board is stored in local storage this method only return the default board that is being used
      resolve(new BoardData(boardId, 'My Board'))
    })
  }

  static _getToolFromLS = function (grid, rawTool) {
    switch (rawTool.type) {
      case ('pen'):
        return new Pen(grid, rawTool.color, rawTool.width)
      case ('eraser'):
        return new Eraser(grid, rawTool.width)
      case ('move'):
        return new Move(grid)
    }
  }

  static _resetLocalStorage = function () {
    const tempGrid = new Grid([], -1)
    window.localStorage.setItem('figures', JSON.stringify(tempGrid.getFiguresArray()))
    window.localStorage.setItem('currFigureId', JSON.stringify(tempGrid.getCurrentFigureId()))
    window.localStorage.setItem('defaultPen', JSON.stringify(new Pen(tempGrid, 'black', 5)))
    window.localStorage.setItem('defaultEraser', JSON.stringify(new Eraser(tempGrid, 20)))
    window.localStorage.setItem('favorites', '[]')
    window.localStorage.setItem('currTool', JSON.stringify(new Pen(tempGrid, 'black', 5)))
    window.localStorage.setItem('canvasSize', JSON.stringify({width: 0, height: 0}))
    window.localStorage.setItem('settings', JSON.stringify([false, false]))
  }
}
