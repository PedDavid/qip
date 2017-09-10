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
      const canvasSize = JSON.parse(window.localStorage.getItem('canvasSize'))
      const initBoard = {
        grid: {figures, maxId: nextFigureId},
        canvasSize
      }

      resolve(initBoard)
    })
  }

  static _cleanCanvasLS = function () {
    this._resetLocalStorage()
  }

  static _updateUserPreferencesLS = function (updatedPreferences) {
    window.localStorage.setItem('favorites', JSON.stringify(updatedPreferences.favorites))
    // window.localStorage.setItem('penColors', JSON.stringify(updatedPreferences.penColors))
    window.localStorage.setItem('defaultPen', JSON.stringify(updatedPreferences.defaultPen))
    window.localStorage.setItem('defaultEraser', JSON.stringify(updatedPreferences.defaultEraser))
    window.localStorage.setItem('currTool', JSON.stringify(updatedPreferences.currTool))
    window.localStorage.setItem('settings', JSON.stringify(updatedPreferences.settings))
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

      const tempCurrTool = JSON.parse(window.localStorage.getItem('currTool'))
      const favorites = JSON.parse(window.localStorage.getItem('favorites'))
        .map(fav => this._getToolFromLS(grid, fav))
      const settings = JSON.parse(window.localStorage.getItem('settings'))
      const userInfo = {
        defaultPen: null,
        defaultEraser: null,
        currTool: this._getToolFromLS(grid, tempCurrTool),
        favorites,
        settings,
        userBoards: [],
        currentBoard: new BoardData(-1, 'My Board', -1)
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
      resolve(new BoardData(boardId, 'My Board', -1))
    })
  }

  static _getToolFromLS = function (grid, rawTool) {
    switch (rawTool.type) {
      case ('pen'):
        return new Pen(grid, rawTool.color, rawTool.width)
      case ('eraser'):
        return new Eraser(grid, rawTool.width, rawTool.eraserType)
      case ('move'):
        return new Move(grid)
    }
  }

  static _sendPenActionLS = function (figure, currentFigureId) {
    // add to localstorage
    const dataFigure = JSON.parse(window.localStorage.getItem('figures'))
    dataFigure.push(figure.exportLS()) // it can be push instead of dataFigure[id] because it will not have crashes with external id's because it's only used when there is no connection
    window.localStorage.setItem('figures', JSON.stringify(dataFigure))
    window.localStorage.setItem('currFigureId', JSON.stringify(currentFigureId))
  }

  static _sendEraserActionLS = function (figureId) {
    // remove from localstorage
    const dataFigure = JSON.parse(window.localStorage.getItem('figures'))
    const figIdx = dataFigure.findIndex(fig => fig.id === figureId)
    dataFigure.splice(figIdx, 1) // use splice (not delete) beacause this way the array updated and reindexed
    window.localStorage.setItem('figures', JSON.stringify(dataFigure))
  }

  static _sendMoveActionLS = function (figure) {
    // move from localstorage
    const dataFigure = JSON.parse(window.localStorage.getItem('figures'))
    const toPersist = figure.exportLS()
    let figIdx = dataFigure.findIndex(f => f.id === toPersist.id)
    dataFigure[figIdx] = toPersist
    window.localStorage.setItem('figures', JSON.stringify(dataFigure))
  }

  static _addClipboardLS = function (figure) {
    window.localStorage.setItem('clipboard', JSON.stringify(figure))    
  }

  static _getClipboardLS = function () {
    return JSON.parse(window.localStorage.getItem('clipboard'))
  }
  static _resetLocalStorage = function () {
    window.localStorage.setItem('figures', '[]')
    window.localStorage.setItem('currFigureId', JSON.stringify(-1))
    window.localStorage.setItem('defaultPen', JSON.stringify(new Pen(null, 'black', 5)))
    window.localStorage.setItem('defaultEraser', JSON.stringify(new Eraser(null, 20)))
    window.localStorage.setItem('favorites', '[]')
    window.localStorage.setItem('currTool', JSON.stringify(new Pen(null, 'black', 5)))
    window.localStorage.setItem('canvasSize', JSON.stringify({width: 0, height: 0}))
    window.localStorage.setItem('settings', JSON.stringify([false, false]))
  }
}
