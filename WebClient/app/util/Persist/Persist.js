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

  connectWS (boardId, accessToken) {
    // TODO(peddavid): Configuration of endpoints
    const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
    const port = document.location.port ? (57059) : ''
    const authorization = accessToken != null ? `?access_token=${accessToken}` : ''
    const connectionUrl = `${scheme}://localhost:${port}/ws/${boardId}${authorization}`
    this.boardId = boardId // this should be here because even if ws connection goes wrong, there is a url to share
    console.log('making web socket connection to: ' + connectionUrl)
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      console.info('web socket connection to: ' + connectionUrl + ' is now open')
      PersistWS._configureWSProtocol(this.socket, this.grid, this.canvasContext)
      this.connected = true
      this.persistType = PersistType().WebSockets
      PersistWS._persistBoardByWS(this.socket)
      PersistLS._resetLocalStorage()
    }

    this.socket.onerror = (event) => {
      console.error('an error occurred on web socket connection to: ' + connectionUrl)
      this.connected = false
    }

    this.socket.onclose = (event) => {
      console.error('WebSocket connection is closing without being handled!! Restarting WebSocket connection ...')
      this.connectWS(boardId, accessToken)
    }
  }

  getInitialBoardAsync (boardId, accessToken) {
    return this.callWSLSFunc(
      () => PersistWS._getInitialBoardWS(boardId, accessToken),
      () => PersistLS._getInitialBoardLS()
    )
  }

  getUserInfoAsync (grid, userProfile, accessToken) {
    return this.callWSLSFunc(
      () => PersistWS._getUserInfoAsyncWS(grid, userProfile, accessToken),
      () => PersistLS._getUserInfoAsyncLS(grid)
    )
  }

  getBoardInfo (boardId, profile, accessToken) {
    return this.callWSLSFunc(
      () => PersistWS._getBoardInfoWS(boardId, profile, accessToken),
      () => PersistLS._getBoardInfoLS(boardId)
    )
  }

  getBoardUsers (boardId, accessToken) {
    return PersistWS._getBoardUsers(boardId, accessToken)
  }

  cleanCanvas () {
    return this.callWSLSFunc(
      () => PersistWS._cleanCanvasWS(this.grid, this.socket),
      () => PersistLS._resetLocalStorage()
    )
  }

  updateUserPreferences (isAuthenticated, updatedPreferences, profile, accessToken) {
    if (isAuthenticated && this.persistType === PersistType().WebSockets) {
      PersistWS._updateUserPreferencesWS(updatedPreferences, profile, accessToken)
    } else {
      PersistLS._updateUserPreferencesLS(updatedPreferences)
    }
  }

  updateCanvasSize (canvasSize) {
    return this.callWSLSFunc(
      () => PersistWS._updateCanvasSizeWS(canvasSize),
      () => PersistLS._updateCanvasSizeLS(canvasSize)
    )
  }

  addUserBoard (boardName, user, accessToken) {
    return PersistWS._addUserBoardWS(boardName, user, accessToken)
  }

  updateBoardBasePermission (boardId, boardName, basePermission, accessToken) {
    return PersistWS._updateBoardBasePermissionWS(boardId, boardName, basePermission, accessToken)
  }

  createUsersPermission (users, boardId, usersPermission, accessToken) {
    return PersistWS._createUsersPermissionWS(users, boardId, usersPermission, accessToken)
  }

  updateUserPermission (userId, boardId, userPermission, accessToken) {
    return PersistWS._updateUserPermissionWS(userId, boardId, userPermission, accessToken)
  }

  getUsersAsync (accessToken) {
    return PersistWS._getUsersWS(accessToken)
  }

  sendPenAction (figure, currentFigureId) {
    if (this.connected) {
      this.socket.send(figure.exportWS())
    } else {
      PersistLS._sendPenActionLS(figure, currentFigureId)
    }
  }

  sendEraserAction (figureId) {
    if (this.connected) {
      const objToSend = {
        type: 'DELETE_LINE',
        payload: {'id': figureId}
      }
      this.socket.send(JSON.stringify(objToSend))
    } else {
      PersistLS._sendEraserActionLS(figureId)
    }
  }

  sendMoveAction (figure, offsetPoint) {
    if (this.connected) {
      this.socket.send(
        figure.exportWS(
          fig => { fig.offsetPoint = offsetPoint }
        ))
    } else {
      PersistLS._sendMoveActionLS(figure)
    }
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
