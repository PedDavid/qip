import React from 'react'
import Hammer from './../../util/Hammer.js'
import {
  Route
} from 'react-router-dom'

import {
  Loader
} from 'semantic-ui-react'

import SideBarOverlay from './components/SideBarOverlay'
import Canvas from './components/Canvas'
import CleanBoardModal from './components/Modals/CleanBoardModal'
import SaveBoardModal from './components/Modals/SaveBoardModal'
import AddBoardModal from './components/Modals/AddBoardModal'
import ShareBoardModal from './components/Modals/ShareBoardModal'
import SettingsModal from './components/Modals/SettingsModal'
import UserAccountModal from './components/Modals/UserAccountModal'
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

const defaultGrid = new Grid([], 0)
const defaultPen = new Pen(defaultGrid, 'black', 5)
const auth = new Auth()
const maxCanvasSize = 3000

export default class Board extends React.Component {
  // check if these default tools are necessary

  persist = {} // this is necessary because the first time render occurs, there is no this.persist object

  state = {
    showCleanModal: false,
    showUserModal: false,
    showShareModal: false,
    showSaveModal: false,
    showAddModal: false,
    showUserAccountModal: false,
    showSettingsModal: false,
    currTool: defaultPen,
    canvasSize: {width: 0, height: 0},
    favorites: [],
    loading: true,
    currentBoard: new BoardData(), // this is necessary because currentBoard is only fetched after first render
    userBoards: [],
    settings: [false, false]
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    // currently, this.perists is not defined but it will on componentDidMount
    onDown: event => this.state.currTool.onPress(event, 1, (x, y) => this.updateCanvasSize(x, y), this.state.settings),
    onUp: event => this.state.currTool.onPressUp(event, this.persist),
    onMove: event => this.state.currTool.onSwipe(event, 1, (x, y) => this.updateCanvasSize(x, y), this.state.settings), // change the way canvas size is updated
    onOut: event => this.state.currTool.onOut(event, this.persist)
  }

  componentDidMount () {
    const boardId = this.props.match.params.board

    this.getInitialBoard(boardId)

    window.addEventListener('resize', event => {
      this.setState(prevState => {
        const newCanvasSize = {
          width: window.innerWidth > this.state.canvasSize.width ? window.innerWidth - 20 : this.state.canvasSize.width,
          height: window.innerHeight > this.state.canvasSize.height ? window.innerHeight - 20 : this.state.canvasSize.height
        }
        this.persist.updateCanvasSize(newCanvasSize)
        return {canvasSize: newCanvasSize}
      })
      // TODO(peddavid): This is probably required to be done after state is changed, setState callback or Update
      this.grid.draw(this.canvasContext, 1)
    })

    // var canvasHammer = new Hammer(this.canvas)

    var canvasHammer = new Hammer.Manager(this.canvas, {
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

  getInitialBoard (boardId) {
    /*
      There is the following possibilities for this initial procedure:
      * User is not authenticated and there is no specific board requested -> persistType is LocalStorage.
      * User is not authenticated but requests a specific board (/board/boardid) -> persistType is WebSockets but profile can't be asked to server.
      * User is authenticated and there is no specific board request -> persistType is WebStorage. Profile is requested to server and currentBoard is obtained from user preferences.
      * User is authenticated but requests a specific board -> persistType is WebSockets. Profile is requested to server but user current board is ignored. It is used requested board instead.
    */
    let persistType = null

    // if there isn't a specific board, or if the user is not authenticated, get persisted data from local storage
    if (boardId != null || auth.isAuthenticated()) {
      persistType = PersistType().WebSockets
    } else if (boardId == null) {
      persistType = PersistType().LocalStorage
    }

    this.persist = new Persist(persistType, this.canvasContext, this.grid)

    const userProfile = auth.isAuthenticated() ? auth.tryGetProfile() : null
    const userAccessToken = auth.isAuthenticated() ? auth.getAccessToken() : null

    // this must be done to process requests when User is not authenticated but requests a specific board
    let getUserInfoPromise = null
    if (persistType === PersistType().WebSockets && !auth.isAuthenticated()) {
      this.persist.persistType = PersistType().LocalStorage
      getUserInfoPromise = this.persist.getUserInfoAsync(this.grid, userProfile, userAccessToken)
      this.persist.persistType = PersistType().WebSockets
    } else {
      getUserInfoPromise = this.persist.getUserInfoAsync(this.grid, userProfile, userAccessToken)
    }

    getUserInfoPromise.then(userinfo => {
      // if there is no pen, eraser or currentBoard
      const defaultPen = new Pen(defaultGrid, 'black', 5)
      const defaultEraser = new Eraser(this.grid, 5)

      // necessary procedure to avoid bug
      this.toolsConfig.updatePrevTool(userinfo.defaultPen != null ? userinfo.defaultPen : defaultPen)
      this.toolsConfig.updatePrevTool(userinfo.defaultEraser != null ? userinfo.defaultEraser : defaultEraser)

      this.setState({
        loading: false,
        currTool: userinfo.currTool != null ? userinfo.currTool : defaultPen,
        favorites: userinfo.favorites != null ? userinfo.favorites : [],
        userBoards: userinfo.userBoards != null ? userinfo.userBoards
          .map(boardRaw => new BoardData(boardRaw.board.id, boardRaw.board.name)) : [],
        settings: userinfo.settings != null ? userinfo.settings : [false, false]
      })
      const userCurrentBoard = userinfo.currentBoard != null ? userinfo.currentBoard : userinfo.userBoards[0].board // return currBoard. todo: change to userinfo.currBoard
      // if user has a predefined current board, use it. In other case, use boardId that was passed in url, if present
      if (boardId != null) {
        return this.persist.getBoardInfo(boardId)
      }
      return userCurrentBoard
    }).then(currBoard => {
      persistType === PersistType().WebSockets && this.props.history.replace('/board/' + currBoard.id)
      this.setState({
        currentBoard: currBoard
      })
      return this.persist.getInitialBoardAsync(boardId == null ? currBoard.id : boardId, userAccessToken)
    }).then(initBoard => {
      this.grid = initBoard.grid
      this.persist.grid = this.grid
      const canvasSize = initBoard.canvasSize
      this.setState({
        canvasSize: {
          width: canvasSize.width === 0 ? window.innerWidth - 20 : canvasSize.width,
          height: canvasSize.height === 0 ? window.innerHeight - 20 : canvasSize.height
        }
      })

      if (this.persist.persistType === PersistType().WebSockets) {
        // todo update board id and start web socket connection
        this.updateBoardId(this.state.currentBoard)
      }

      // draw initial grid
      this.grid.draw(this.canvasContext, 1)
    }).catch(err => {
      console.error(err)
      console.log(err.message)
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
      this.persist.updateFavorites(this.state.favorites)
    }) // not needed to change prevState
  }
  removeFavorite = (tool) => {
    this.setState((prevState) => {
      const index = prevState.favorites.indexOf(tool)
      if (index > -1) {
        prevState.favorites.splice(index, 1)
      }
      this.persist.updateFavorites(prevState.favorites)
    })
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
      this.persist.updateFavorites(prevState.favorites)
    })
  }
  drawImage = (imageSrc) => {
    const newImage = new Image({x: 80, y: 80}, imageSrc)
    // do not change this order. image must be added to grid first to set the new id
    this.grid.addImage(newImage)
    newImage.persist(this.persist, this.grid)
    this.grid.draw(this.canvasContext, 1)
  }
  changeCurrentTool = (tool) => {
    this.toolsConfig.updatePrevTool(this.state.currTool)
    this.persist.updateCurrTool(tool)
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

  toggleShareModal = () => {
    this.setState(prevState => ({showShareModal: !prevState.showShareModal}))
  }

  toggleSaveModal = () => {
    this.setState(prevState => { return { showSaveModal: !prevState.showSaveModal } })
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

  refCallback = (ref) => {
    this.canvas = ref.canvas
    this.canvasContext = ref.canvas.getContext('2d')
  }

  updateBoardId = (board) => {
    console.info('current board id: ' + this.props.match.params.board)
    this.persist.connectWS(board.id)
  }

  addBoardAsync = (boardName) => {
    return this.persist.addUserBoard(boardName, auth.tryGetProfile(), auth.getAccessToken())
      .then(addedBoard => {
        this.setState(prevState => {
          const newBoard = new BoardData(addedBoard.id, addedBoard.name)
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

  render () {
    return (
      <div ref='maindiv' onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.boardStyle} style={{width: this.state.canvasSize.width, height: this.state.canvasSize.height}}>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite}
          removeFavorite={this.removeFavorite} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}
          toggleSaveModal={this.toggleSaveModal} drawImage={this.drawImage} canvasSize={this.state.canvasSize} auth={auth}
          addBoard={this.toggleAddModal} currentBoard={this.state.currentBoard} userBoards={this.state.userBoards} persist={this.persist}
          openUserAccount={this.toggleUserAccountModal} moveFavorite={this.moveFavorite} openSettings={this.toggleSettingsModal}>
          <Canvas ref={this.refCallback} width={this.state.canvasSize.width} height={this.state.canvasSize.height} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} closeModal={this.toggleCleanModal} visible={this.state.showCleanModal} />
        <ShareBoardModal location={this.props.location} history={this.props.history} persist={this.persist}
          visible={this.state.showShareModal} closeModal={this.toggleShareModal} updateCurrentBoard={this.updateBoardId} />
        <Loader active={this.state.loading} content='Fetching Data ...' />
        <Route exact path='/callback' render={props => {
          return <Callback auth={auth} {...props} />
        }} />
        <SaveBoardModal history={this.props.history} persist={this.persist} visible={this.state.showSaveModal} closeModal={this.toggleSaveModal} auth={auth}
          updateCurrentBoard={this.updateBoardId} />
        {/* todo: see if it's possibly to merge saveBoardModal and AddBoardModal */}
        <AddBoardModal history={this.props.history} persist={this.persist} visible={this.state.showAddModal} closeModal={this.toggleAddModal} auth={auth}
          addBoardAsync={this.addBoardAsync} />
        <UserAccountModal auth={auth} visible={this.state.showUserAccountModal} closeModal={this.toggleUserAccountModal} />
        <SettingsModal settings={this.state.settings} updateSettings={this.updateSettings} visible={this.state.showSettingsModal} closeModal={this.toggleSettingsModal} />
      </div>
    )
  }
}
