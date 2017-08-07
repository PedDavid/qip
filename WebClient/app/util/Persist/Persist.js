import PersistLS from './PersistLS.js'
import PersistWS from './PersistWS.js'

export class Persist {
  // todo: check if it's necessary all parameters
  constructor (persistType: PersistType, canvasContext, grid) {
    this.persistType = persistType
    this.canvasContext = canvasContext
    this.socket = null
    this.grid = grid
    this.connected = false
    this.boardId = null // connected board
  }

  connectWS (boardId) {
    // TODO(peddavid): Configuration of endpoints
    const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
    const port = document.location.port ? (57059) : ''
    const connectionUrl = `${scheme}://localhost:${port}/ws/${boardId}`
    console.log('making web socket connection to: ' + connectionUrl)
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      console.info('web socket connection to: ' + connectionUrl + ' is now open')
      PersistWS._configureWSProtocol(this.socket, this.grid, this.canvasContext)
      this.connected = true
      this.boardId = boardId
      this.persistType = PersistType().WebSockets
      PersistWS._persistBoardByWS(this.socket)
      PersistLS._resetLocalStorage()
    }

    this.socket.onerror = (event) => {
      console.error('an error occurred on web socket connection to: ' + connectionUrl)
      this.connected = false
      this.boardId = null
    }
  }

  getInitialBoardAsync (boardId) {
    return this.callWSLSFunc(
      () => PersistWS._getInitialBoardWS(boardId),
      () => PersistLS._getInitialBoardLS()
    )
  }

  getUserInfoAsync (grid, userProfile) {
    return this.callWSLSFunc(
      () => PersistWS._getUserInfoAsyncWS(grid, userProfile),
      () => PersistLS._getUserInfoAsyncLS(grid)
    )
  }

  getBoardInfo (boardId) {
    return this.callWSLSFunc(
      () => PersistWS._getBoardInfoWS(boardId),
      () => PersistLS._getBoardInfoLS(boardId)
    )
  }

  cleanCanvas () {
    return this.callWSLSFunc(
      () => PersistWS._cleanCanvasWS(this.grid, this.socket),
      () => PersistLS._resetLocalStorage()
    )
  }

  updateFavorites (newFavs) {
    return this.callWSLSFunc(
      () => PersistWS._updateFavoritesWS(newFavs),
      () => PersistLS._updateFavoritesLS(newFavs)
    )
  }

  updateCurrTool (newCurrTool) {
    return this.callWSLSFunc(
      () => PersistWS._updateFavoritesWS(newCurrTool),
      () => PersistLS._updateFavoritesLS(newCurrTool)
    )
  }

  addUserBoard () {

  }

  callWSLSFunc (WSFunc, LSFunc) {
    if (this.persistType === PersistType().WebSockets) {
      return WSFunc()
    } else if (this.persistType === PersistType().LocalStorage) {
      return LSFunc()
    }
  }
}

export function PersistType () {
  return {
    WebSockets: 0,
    LocalStorage: 1
  }
}