import React from 'react'
import Hammer from './../../util/Hammer.js'
import {
  Route
} from 'react-router-dom'

import {
  Loader,
  Message
} from 'semantic-ui-react'

import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import CleanBoardModal from './components/Modals/CleanBoardModal'
import AddBoardModal from './components/Modals/AddBoardModal'
import ShareBoardModal from './components/Modals/ShareBoardModal'
import ImportImageModal from './components/Modals/ImportImageModal'
import SettingsModal from './components/Modals/SettingsModal'
import UserAccountModal from './components/Modals/UserAccountModal'
import UsersManagementModal from './components/Modals/UsersManagementModal'
import styles from './styles.scss'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'
import BoardData from './../../model/BoardData'
import {Image} from './../../model/Image'
import ToolsConfig from './../../model/ToolsConfig'
import defaultToolsConfig from './../../public/configFiles/defaultTools'
import {Persist, PersistType} from './../../util/Persist/Persist'
import Auth from './../../auth/Auth'
import Callback from './../../auth/Callback/SignInCallback.js'
import SettingsConfig from './../../util/SettingsConfig.js'
import ContextMenu from './components/ContextMenu'

import { uploadImage } from './../../services/imgur'

const maxCanvasSize = 3000

export default class Board extends React.Component {
  // check if these default tools are necessary
  grid = new Grid([], 0)
  auth = new Auth(() => this.getInitialBoard(null), this.props.history) // this lambda may not be the best solution
  persist = new Persist(null, null, null) // this is necessary because the first time render occurs, there is no this.persist object
  maxErrorCount = 3

  state = {
    showCleanModal: false,
    showUserModal: false,
    showShareModal: false,
    showImportImageModal: false,
    showAddModal: false,
    showUserAccountModal: false,
    showSettingsModal: false,
    showUsersManagementModal: false,
    currTool: null,
    canvasSize: {width: 0, height: 0},
    favorites: [],
    loading: true,
    currentBoard: new BoardData(), // this is necessary because currentBoard is only fetched after first render
    userBoards: [],
    settings: [false, false],
    notification: null,
    contextMenuVisibility: false
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    // currently, this.perists is not defined but it will on componentDidMount
    onDown: event => this.state.currTool.onPress(event, 1, (x, y) => this.updateCanvasSize(x, y), this.state.settings),
    onUp: event => this.state.currTool.onPressUp(event, this.persist),
    onMove: event => this.state.currTool.onSwipe(event, 1, (x, y) => this.updateCanvasSize(x, y), this.state.settings), // change the way canvas size is updated
    onOut: event => this.state.currTool.onOut(event, this.persist),
    onContextMenu: event => this.state.currTool.onContextMenu(event, this.persist, this.openContextMenu, this.closeContextMenu, this.canvasContext)
  }

  componentDidMount () {
    const boardId = this.props.match.params.board

    this.getInitialBoard(boardId)

    window.addEventListener('resize', event => {
      this.setState(prevState => {
        const newCanvasSize = {
          width: window.innerWidth > this.state.canvasSize.width || !prevState.settings[SettingsConfig.dynamicPageSettingIdx] ? window.innerWidth - 20 : this.state.canvasSize.width,
          height: window.innerHeight > this.state.canvasSize.height || !prevState.settings[SettingsConfig.dynamicPageSettingIdx] ? window.innerHeight - 20 : this.state.canvasSize.height
        }
        this.persist.updateCanvasSize(newCanvasSize)
        return {canvasSize: newCanvasSize}
      }, () => this.grid.draw(this.canvasContext, 1))
    })

    const canvasHammer = new Hammer.Manager(this.canvas, {
      inputClass: Hammer.TouchInput, // only touch triggers hammer js
      recognizers: [
        // RecognizerClass, [options], [recognizeWith, ...], [requireFailure, ...]
        [Hammer.Pan, { direction: Hammer.DIRECTION_ALL }]
      ]
    })

    canvasHammer.on('pan', ev => {
      if (this.state.settings[SettingsConfig.fingerMoveSettingIdx]) {
        window.scrollTo(window.scrollX - ev.deltaX / 2, window.scrollY - ev.deltaY / 2)
      }
    })
  }

  componentWillUnmount () {
    this.persist.closeConnection()
  }

  getInitialBoard = (boardId) => {
    /*
      There is the following possibilities for this initial procedure:
      * User is not authenticated and there is no specific board requested -> persistType is LocalStorage.
      * User is not authenticated but requests a specific board (/board/boardid) -> persistType is WebSockets but profile can't be asked to server.
      * User is authenticated and there is no specific board request -> persistType is WebStorage. Profile is requested to server and currentBoard is obtained from user preferences.
      * User is authenticated but requests a specific board -> persistType is WebSockets. Profile is requested to server but user current board is ignored. It is used requested board instead.
    */
    this.setState({
      loading: true
    })
    let persistType = null

    // if there isn't a specific board, or if the user is not authenticated, get persisted data from local storage
    if (boardId != null || this.auth.isAuthenticated()) {
      persistType = PersistType().WebSockets
    } else if (boardId == null) {
      persistType = PersistType().LocalStorage
    }

    this.persist = new Persist(persistType, this.canvasContext, this.grid)

    const userProfile = this.auth.isAuthenticated() ? this.auth.tryGetProfile() : null
    const userAccessToken = this.auth.isAuthenticated() ? this.auth.getAccessToken() : null

    // this must be done to process requests when User is not authenticated but requests a specific board
    let getUserInfoPromise = null
    if (persistType === PersistType().WebSockets && !this.auth.isAuthenticated()) {
      this.persist.persistType = PersistType().LocalStorage
      getUserInfoPromise = this.persist.getUserInfoAsync(this.grid, userProfile, userAccessToken)
      this.persist.persistType = PersistType().WebSockets
    } else {
      getUserInfoPromise = this.persist.getUserInfoAsync(this.grid, userProfile, userAccessToken)
    }

    getUserInfoPromise.then(userinfo => {
      // if there is no pen, eraser or currentBoard
      const defaultPen = new Pen(this.grid, 'black', 5)
      const defaultEraser = new Eraser(this.grid, 5)

      // necessary procedure to avoid bug
      this.toolsConfig.updatePrevTool(userinfo.defaultPen != null ? userinfo.defaultPen : defaultPen)
      this.toolsConfig.updatePrevTool(userinfo.defaultEraser != null ? userinfo.defaultEraser : defaultEraser)

      this.setState({
        currTool: userinfo.currTool != null ? userinfo.currTool : defaultPen,
        favorites: userinfo.favorites != null ? userinfo.favorites : [],
        userBoards: userinfo.userBoards != null ? userinfo.userBoards
          .map(boardRaw => new BoardData(boardRaw.board.id, boardRaw.board.name, boardRaw.board.basePermission, boardRaw.permission)) : [],
        settings: userinfo.settings != null ? userinfo.settings : [false, false]
      })
      // if user just authenticated and had some figures in localstorage, create a new board and when websocket connection is oppened, localstorage's figures will be sent to server
      if (JSON.parse(window.localStorage.getItem('figures')).length > 0 && this.auth.isAuthenticated()) {
        return this.persist.addUserBoard('Local Board Replied', userProfile, userAccessToken)
      }
      // if user has a predefined current board, use it. However, if it was passed a boardId in url, use it instead
      if (boardId != null) {
        return this.persist.getBoardInfo(boardId, userProfile, userAccessToken)
      }
      // check if user has not currentBoard predefined. If not, create one
      const adminBoards = this.state.userBoards.filter(board => board.userPermission === 3)
      if (userinfo.currentBoard == null && adminBoards.length === 0) { // remove userBoards.length check when userinfo from ws comes with currentBoard
        const newBoard = this.persist.addUserBoard('My First Board', userProfile, userAccessToken)
        this.setState({userBoards: [newBoard]})
        return newBoard
      }
      return userinfo.currentBoard != null ? userinfo.currentBoard : adminBoards[0] // return currBoard. todo: change to userinfo.currBoard
    }).then(currBoard => {
      persistType === PersistType().WebSockets && this.props.history.replace('/board/' + currBoard.id)
      this.setState({
        currentBoard: currBoard
      })
      if (currBoard.userPermission === 1) {
        window.alert('All modifications to board you may do, will not be persisted as you only have view permissions to this board')
      } else if (currBoard.userPermission === 0) {
        window.alert('You do not have public access to this board!. Redirecting to home board ...')
      }
      return this.persist.getInitialBoardAsync(boardId == null ? currBoard.id : boardId, userAccessToken)
    }).then(initBoard => {
      // update this.grid
      this.grid.addInitialFigures(initBoard.grid.figures, this.canvasContext)
      this.grid.setCurrentFigId(initBoard.grid.maxId)

      const canvasSize = initBoard.canvasSize

      if (this.persist.persistType === PersistType().WebSockets) {
        // todo update board id and start web socket connection
        this.updateBoardId(this.state.currentBoard)
      }

      this.setState(prevState => {
        return {
          loading: false,
          canvasSize: {
            width: canvasSize.width === 0 || window.innerWidth > canvasSize.width ? window.innerWidth - 20 : canvasSize.width,
            height: canvasSize.height === 0 || window.innerHeight > canvasSize.height ? window.innerHeight - 20 : canvasSize.height
          }
        }
      })

      // draw initial grid
      this.grid.draw(this.canvasContext, 1)
    }).catch(err => {
      console.error(err)
      console.log(err.message)
      if (this.maxErrorCount <= 0) {
        this.auth.logout()
        return
      }
      this.maxErrorCount --
      // this must be done because when an error occurs and history is set, initialBoard
      // is not set anymore
      this.getInitialBoard(null) // get initial board from Local Storage
      this.setState({
        loading: false
      })
      this.props.history.push('/') // change current location programmatically in case of error
    })
  }

  addFavorite = (tool) => {
    this.setState(() => {
      this.state.favorites.push(tool)
      this.updateUserPreferences('favorites', this.state.favorites)
    }) // not needed to change prevState
  }
  removeFavorite = (tool) => {
    this.setState((prevState) => {
      const index = prevState.favorites.indexOf(tool)
      if (index > -1) {
        prevState.favorites.splice(index, 1)
      }
      this.updateUserPreferences('favorites', prevState.favorites)
    })
  }
  updateUserPreferences = (preferenceNameToUpdate, updatedPreference) => {
    const updatedPreferences = {
      favorites: this.state.favorites,
      penColors: null,
      defaultPen: this.toolsConfig['pen'].lastValue,
      defaultEraser: this.toolsConfig['eraser'].lastValue,
      currTool: this.state.currTool,
      settings: this.state.settings
    }
    updatedPreferences[preferenceNameToUpdate] = updatedPreference
    this.persist.updateUserPreferences(this.auth.isAuthenticated(), updatedPreferences, this.auth.tryGetProfile(), this.auth.getAccessToken())
  }
  moveFavorite = (tool, movingUp) => {
    this.setState((prevState) => {
      let index = -1
      prevState.favorites
        .find((mtool, idx) => { // array.findIndex was not working, possible because tool.grid was not equal
          if (mtool.equals(tool)) {
            index = idx
          }
        })

      if (index === -1 || tool == null) {
        return
      } else if (movingUp && index - 1 >= 0) {
        const auxTool = prevState.favorites[index - 1]
        prevState.favorites[index - 1] = tool
        prevState.favorites[index] = auxTool
      } else if (!movingUp && index + 1 <= prevState.favorites.length) {
        const auxTool = prevState.favorites[index + 1]
        prevState.favorites[index + 1] = tool
        prevState.favorites[index] = auxTool
      }
      this.updateUserPreferences('favorites', prevState.favorites)
    })
  }
  changeCurrentTool = (tool) => {
    this.toolsConfig.updatePrevTool(this.state.currTool)
    this.updateUserPreferences('currTool', tool)
    this.setState({currTool: tool})
  }
  cleanCanvas = () => {
    this.persist.cleanCanvas()
    this.grid.clean(this.canvasContext)
    this.toggleCleanModal()
    this.resetCanvasSize()
  }
  resetCanvasSize = () => {
    this.setState(prevState => {
      const resetedCanvasSize = {
        width: window.innerWidth - 20,
        height: window.innerHeight - 20
      }
      this.persist.updateCanvasSize(resetedCanvasSize)
      return {canvasSize: resetedCanvasSize}
    })
  }
  updateCanvasSize = (x, y) => {
    // check if dynamic page setting is checked
    if (!this.state.settings[SettingsConfig.dynamicPageSettingIdx]) {
      return
    }
    let updated = false
    this.setState(prevState => {
      let prevCanvasSize = prevState.canvasSize
      // check if event occurred in trigger zone and if canvas size can be augmented
      if (x > prevState.canvasSize.width - 100 && prevCanvasSize.width + 300 < maxCanvasSize) {
        prevCanvasSize.width += 300
        updated = true
      } if (y > prevState.canvasSize.height - 100 && prevCanvasSize.height + 300 < maxCanvasSize) {
        prevCanvasSize.height += 300
        updated = true
      }
      return {canvasSize: prevCanvasSize}
    })
    if (updated) {
      this.grid.draw(this.canvasContext, 1)
      this.persist.updateCanvasSize(this.state.canvasSize)
    }
  }
  toggleCleanModal = () => {
    this.setState(prevState => ({showCleanModal: !prevState.showCleanModal}))
  }

  onImageLoad = (imageSrc) => {
    const newImage = new Image({x: 80, y: 80}, imageSrc, null, null, null, image => {
      this.grid.addImage(image)
      this.grid.draw(this.canvasContext, 1)
    })
    uploadImage(imageSrc)
      .then(res => {
        console.info(`Image uploaded to ${res.data.link}`)
        this.notifySuccess(`Image uploaded to imgur`)
        newImage.setImageSrc(res.data.link)
        newImage.persist(this.persist, this.grid)
      })
      .catch(() => {
        this.grid.removeImage(newImage.id, this.canvasContext, 1, false)
        this.notifyError('Error uploading image to imgur')
      })
  }
  toggleImportImageModal = () => {
    this.setState(prevState => ({showImportImageModal: !prevState.showImportImageModal}))
  }

  toggleShareModal = () => {
    this.setState(prevState => ({showShareModal: !prevState.showShareModal}))
  }

  toggleAddModal = () => {
    this.setState(prevState => { return { showAddModal: !prevState.showAddModal } })
  }

  toggleUserAccountModal = () => {
    this.setState(prevState => { return { showUserAccountModal: !prevState.showUserAccountModal } })
  }

  toggleSettingsModal = () => {
    this.setState(prevState => { return { showSettingsModal: !prevState.showSettingsModal } })
  }

  toggleUsersManagementModal = () => {
    this.setState(prevState => { return { showUsersManagementModal: !prevState.showUsersManagementModal } })
  }

  refCallback = (ref) => {
    this.canvas = ref.canvas
    this.canvasContext = ref.canvas.getContext('2d')
  }

  updateBoardId = (board) => {
    console.info('current board id: ' + this.props.match.params.board)
    this.persist.connectWS(board.id, this.auth.getAccessToken())
  }

  addBoardAsync = (boardName) => {
    const userPermission = this.auth.isAuthenticated ? 3 : 2 // if there is a user authenticated, that user is admin. In other cases, it has edit permissions
    return this.persist.addUserBoard(boardName, this.auth.tryGetProfile(), this.auth.getAccessToken())
      .then(addedBoard => {
        this.setState(prevState => {
          const newBoard = new BoardData(addedBoard.id, addedBoard.name, addedBoard.basePermission, userPermission)
          prevState.userBoards.push(newBoard)
        })
        return addedBoard
      })
  }

  updateSettings = (setting, idx) => {
    this.setState(prevState => {
      prevState.settings[idx] = setting
    })
    // todo: persist settings
  }

  undo = () => {
    this.grid.undo(this.canvasContext, this.persist)
  }

  notifyError = (message) => {
    this.setState({notification: {props: {error: true}, message}}, () => setTimeout(this.dismissNotification, 3000))
  }
  notifySuccess = (message) => {
    this.setState({notification: {props: {success: true}, message}}, () => setTimeout(this.dismissNotification, 2000))
  }

  dismissNotification = () => {
    this.setState({notification: null})
  }

  openContextMenu = (clientX, clientY, contextMenuRaw) => {
    this.clientX = clientX
    this.clientY = clientY
    this.contextMenuRaw = contextMenuRaw
    this.setState({contextMenuVisibility: true})
  }

  closeContextMenu = () => {
    this.setState({contextMenuVisibility: false})
  }

  removeBoard = (boardId) => {
    if (this.state.currentBoard.id === boardId) {
      return window.alert('You cannot delete your current board!')
    }
    this.setState(prevState => {
      var index = prevState.userBoards.findIndex(board => board.id === boardId)
      if (index > -1) {
        prevState.userBoards.splice(index, 1)
      }
    })
    const isOwner = this.state.userBoards.find(board => board.id === boardId).userPermission === 3
    this.persist.removeBoard(boardId, this.auth.tryGetProfile(), this.auth.getAccessToken(), isOwner)
  }

  render () {
    return (
      <div className={styles.boardStyle} style={{width: this.state.canvasSize.width, height: this.state.canvasSize.height}}>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite}
          removeFavorite={this.removeFavorite} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}
          onImageLoad={this.toggleImportImageModal} canvasSize={this.state.canvasSize} auth={this.auth} changeCurrentBoard={this.getInitialBoard}
          addBoard={this.toggleAddModal} currentBoard={this.state.currentBoard} userBoards={this.state.userBoards} persist={this.persist}
          openUserAccount={this.toggleUserAccountModal} moveFavorite={this.moveFavorite} openSettings={this.toggleSettingsModal}
          undo={this.undo} toggleUsersManagementModal={this.toggleUsersManagementModal} removeBoard={this.removeBoard} closeContextMenu={this.closeContextMenu}
          openContextMenu={this.openContextMenu}>
          <Canvas ref={this.refCallback} width={this.state.canvasSize.width} height={this.state.canvasSize.height} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
          {this.state.notification !== null &&
            <Message style={{position: 'fixed', textAlign: 'center', bottom: '0'}} onDismiss={this.dismissNotification} {...this.state.notification.props}>
              {this.state.notification.message}
            </Message>
          }
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} onClose={this.toggleCleanModal} open={this.state.showCleanModal} />
        <ImportImageModal onClose={this.toggleImportImageModal} open={this.state.showImportImageModal} onImageLoad={this.onImageLoad} />
        <ShareBoardModal location={this.props.location} history={this.props.history} persist={this.persist}
          visible={this.state.showShareModal} onClose={this.toggleShareModal} updateCurrentBoard={this.updateBoardId}
          addBoardAsync={this.addBoardAsync} auth={this.auth} currentBoard={this.state.currentBoard} getInitialBoard={this.getInitialBoard} />
        <Loader active={this.state.loading} content='Fetching Data ...' />
        <Route exact path='/callback' render={props => {
          return <Callback auth={this.auth} {...props} />
        }} />
        <AddBoardModal history={this.props.history} persist={this.persist} visible={this.state.showAddModal} closeModal={this.toggleAddModal} auth={this.auth}
          addBoardAsync={this.addBoardAsync} />
        <UserAccountModal auth={this.auth} visible={this.state.showUserAccountModal} closeModal={this.toggleUserAccountModal} />
        <SettingsModal settings={this.state.settings} updateSettings={this.updateSettings} visible={this.state.showSettingsModal} closeModal={this.toggleSettingsModal} />
        <UsersManagementModal visible={this.state.showUsersManagementModal} closeModal={this.toggleUsersManagementModal} persist={this.persist} auth={this.auth} />
        <ContextMenu canvasSize={this.state.canvasSize} visible={this.state.contextMenuVisibility} top={this.clientY} left={this.clientX} contextMenuRaw={this.contextMenuRaw} />
      </div>
    )
  }
}
