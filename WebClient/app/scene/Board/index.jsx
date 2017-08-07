import React from 'react'

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

const defaultGrid = new Grid([], 0)
const defaultPen = new Pen(defaultGrid, 'black', 5)
const auth = new Auth()

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
    currTool: defaultPen,
    canvasSize: {width: 0, height: 0},
    favorites: [],
    loading: true,
    currentBoard: new BoardData(), // this is necessary because currentBoard is only fetched after first render
    userBoards: []
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    // currently, this.perists is not defined but it will on componentDidMount
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event, this.persist),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event, this.persist)
  }

  componentWillMount () {
    console.log(window.innerWidth)
    console.log(window.innerHeight)
    this.setState({
      canvasSize: {
        width: window.innerWidth,
        height: window.innerHeight
      }
    })
  }

  componentDidMount () {
    const boardId = this.props.match.params.board

    this.getInitialBoard(boardId)

    window.addEventListener('resize', event => {
      this.setState({
        canvasSize: {
          width: window.innerWidth,
          height: window.innerHeight
        }
      })
      this.grid.draw(this.canvasContext, 1)
    })
  }

  getInitialBoard (boardId) {
    let persistType = null

    // if there isn't a specific board, or if the user is not authenticated, get persisted data from local storage
    // todo: implement isAuthenticated()
    if (boardId == null) {
      persistType = PersistType().LocalStorage
    } else if (boardId != null) {
      persistType = PersistType().WebSockets
    }

    this.persist = new Persist(persistType, this.canvasContext, this.grid)

    // get initial board from server or from local storage
    this.persist.getBoardInfo(boardId)
      .then(currBoard => {
        this.setState({
          currentBoard: currBoard
        })
        return this.persist.getInitialBoardAsync(boardId)
      })
      .then(initBoard => {
        this.grid = initBoard.grid
        this.persist.grid = this.grid

        if (this.persist.persistType === PersistType().WebSockets) {
          this.updateBoardId(this.state.currentBoard)
        }

        // draw initial grid
        this.grid.draw(this.canvasContext, 1)

        return this.persist.getUserInfoAsync(this.grid, auth.tryGetProfile())
      })
      .then(userinfo => {
        // if there is no pen and eraser
        const defaultPen = new Pen(this.grid, 'black', 5)
        const defaultEraser = new Eraser(this.grid, 5)

        // necessary procedure to avoid bug
        this.toolsConfig.updatePrevTool(userinfo.defaultPen != null ? userinfo.defaultPen : defaultPen)
        this.toolsConfig.updatePrevTool(userinfo.defaultEraser != null ? userinfo.defaultEraser : defaultEraser)

        this.setState({
          loading: false,
          currTool: userinfo.currTool != null ? userinfo.currTool : defaultPen,
          favorites: userinfo.favorites != null ? userinfo.favorites : [],
          userBoards: userinfo.userBoards != null ? userinfo.userBoards : []
        })
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
  }
  toggleCleanModal = () => {
    this.setState(prevState => { return { showCleanModal: !prevState.showCleanModal } })
  }

  toggleShareModal = () => {
    this.setState(prevState => { return { showShareModal: !prevState.showShareModal } })
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

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  updateBoardId = (board) => {
    console.info('current board id: ' + this.props.match.params.board)
    this.persist.connectWS(board.id)
  }

  addBoard = (board) => {
    this.setState(prevState => {
      prevState.userBoards.push(board)
      return {
        currBoard: board
      }
    })
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite}
          removeFavorite={this.removeFavorite} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}
          toggleSaveModal={this.toggleSaveModal} drawImage={this.drawImage} canvasSize={this.state.canvasSize} auth={auth}
          addBoard={this.toggleAddModal} currentBoard={this.state.currentBoard} userBoards={this.state.userBoards} persist={this.persist}
          openUserAccount={this.toggleUserAccountModal}>
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
          addBoard={this.addBoard} />
        <UserAccountModal auth={auth} visible={this.state.showUserAccountModal} closeModal={this.toggleUserAccountModal} />
      </div>
    )
  }
}
