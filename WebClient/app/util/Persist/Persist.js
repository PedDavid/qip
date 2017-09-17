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
    const authorization = accessToken != null ? `?access_token=${accessToken}` : ''
    const connectionUrl = `${scheme}://qipserverapi.azurewebsites.net/ws/${boardId}${authorization}`
    this.boardId = boardId // this should be here because even if ws connection goes wrong, there is a url to share
    console.log('making web socket connection to: ' + connectionUrl)
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      console.info('web socket connection to: ' + connectionUrl + ' is now open')
      PersistWS._configureWSProtocol(this.socket, this.grid, this.canvasContext)
      this.connected = true
      this.persistType = PersistType().WebSockets
      PersistWS._persistBoardByWS(this.socket, boardId)
      PersistLS._resetLocalStorage()
    }

    this.socket.onerror = (event) => {
      console.error('an error occurred on web socket connection to: ' + connectionUrl)
      console.log(event)
      this.connected = false
    }

    this.socket.onclose = (event) => {
      let reason
      // See http://tools.ietf.org/html/rfc6455#section-7.4.1
      if (event.code === 1000) {
        reason = 'Normal closure, meaning that the purpose for which the connection was established has been fulfilled.'
      } else if (event.code === 1001) {
        reason = 'An endpoint is "going away", such as a server going down or a browser having navigated away from a page.'
      } else if (event.code === 1002) {
        reason = 'An endpoint is terminating the connection due to a protocol error'
      } else if (event.code === 1003) {
        reason = 'An endpoint is terminating the connection because it has received a type of data it cannot accept (e.g., an endpoint that understands only text data MAY send this if it receives a binary message).'
      } else if (event.code === 1004) {
        reason = 'Reserved. The specific meaning might be defined in the future.'
      } else if (event.code === 1005) {
        reason = 'No status code was actually present.'
      } else if (event.code === 1006) {
        reason = 'The connection was closed abnormally, e.g., without sending or receiving a Close control frame'
      } else if (event.code === 1007) {
        reason = 'An endpoint is terminating the connection because it has received data within a message that was not consistent with the type of the message (e.g., non-UTF-8 [http://tools.ietf.org/html/rfc3629] data within a text message).'
      } else if (event.code === 1008) {
        reason = 'An endpoint is terminating the connection because it has received a message that "violates its policy". This reason is given either if there is no other sutible reason, or if there is a need to hide specific details about the policy.'
      } else if (event.code === 1009) {
        reason = 'An endpoint is terminating the connection because it has received a message that is too big for it to process.'
      } else if (event.code === 1010) { // Note that this status code is not used by the server, because it can fail the WebSocket handshake instead.
        reason = 'An endpoint (client) is terminating the connection because it has expected the server to negotiate one or more extension, but the server didn\'t return them in the response message of the WebSocket handshake. <br /> Specifically, the extensions that are needed are: ' + event.reason
      } else if (event.code === 1011) {
        reason = 'A server is terminating the connection because it encountered an unexpected condition that prevented it from fulfilling the request.'
      } else if (event.code === 1015) {
        reason = 'The connection was closed due to a failure to perform a TLS handshake (e.g., the server certificate can\'t be verified).'
      } else {
        reason = 'Unknown reason'
      }
      console.info('WebSocket connection is closing ...')
      console.info(reason)
      this.connectWS(boardId, accessToken)
    }
  }

  closeConnection () {
    this.socket.close()
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
      () => PersistWS._cleanCanvasWS(this.boardId, this.socket),
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
    console.log('sending pen action')
    if (this.connected) {
      this.socket.send(
        figure.exportWS(
          'CREATE_LINE',
          this.boardId,
          (fig) => {
            fig.tempId = fig.id
            delete fig.id
          }
        ))
    } else {
      PersistLS._sendPenActionLS(figure, currentFigureId)
    }
  }

  sendEraserAction (figureId) {
    console.log('sending eraser action')
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

  removeImage (imageId) {
    console.log('sending remove image')
    if (this.connected) {
      const objToSend = {
        type: 'DELETE_IMAGE',
        payload: {'id': imageId}
      }
      this.socket.send(JSON.stringify(objToSend))
    } else {
      PersistLS._sendEraserActionLS(imageId)
    }
  }

  sendMoveAction (figure, offsetPoint, isScaling, type) {
    console.log('sending move action')
    if (this.connected) {
      this.socket.send(
        figure.exportWS(
          type === 'figure' ? 'ALTER_LINE' : 'ALTER_IMAGE',
          this.boardId,
          fig => {
            fig.offsetPoint = {X: offsetPoint.x, Y: offsetPoint.y}
            fig.isScaling = isScaling
            fig.isClosed = false
          }
        ))
    } else {
      PersistLS._sendMoveActionLS(figure)
    }
  }

  addClipboard (figure) {
    PersistLS._addClipboardLS(figure)
  }

  getClipboard () {
    return PersistLS._getClipboardLS()
  }

  sendPointer (pointer) {
    console.log('sending pointer')
    if (this.connected) {
      const objToSend = {
        type: 'POINT_TO',
        payload: pointer
      }
      this.socket.send(JSON.stringify(objToSend))
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
