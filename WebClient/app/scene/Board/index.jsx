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
import EnterUserModal from './components/Modals/EnterUserModal'
import ShareBoardModal from './components/Modals/ShareBoardModal'
import styles from './styles.scss'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'
import ToolsConfig from './../../model/ToolsConfig'
import defaultToolsConfig from './../../public/configFiles/defaultTools'
import {Persist, PersistType} from './../../util/Persist'
// import {Figure, FigureStyle} from './../../model/Figure'

const defaultGrid = new Grid([], 0)
const defaultPen = new Pen(defaultGrid, 'black', 5)

export default class Board extends React.Component {
  // check if these default tools are necessary

  persist = {} // this is necessary because the first time render occurs, there is no this.persist object

  state = {
    showCleanModal: false,
    showUserModal: false,
    showShareModal: false,
    currTool: defaultPen,
    favorites: [], // obtain favorites from server
    loading: true
  }
  toolsConfig = new ToolsConfig(defaultToolsConfig)

  listeners = {
    // currently, this.perists is not defined but it will on componentDidMount
    onDown: event => this.state.currTool.onPress(event, 1),
    onUp: event => this.state.currTool.onPressUp(event, this.persist),
    onMove: event => this.state.currTool.onSwipe(event, 1),
    onOut: event => this.state.currTool.onOut(event, this.persist)
  }

  componentDidMount () {
    const boardId = this.props.match.params.board

    this.getInitialBoard(boardId)
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
    this.persist.getInitialBoardAsync(boardId)
      .then(grid => {
        this.grid = grid
        this.persist.grid = this.grid

        if (this.persist.persistType === PersistType().WebSockets) {
          // todo update board id and start web socket connection
          this.updateBoardId(boardId)
        }

        // draw initial grid
        this.grid.draw(this.canvasContext, 1)

        // if there is no pen and eraser
        const defaultPen = new Pen(this.grid, 'black', 5)
        const defaultEraser = new Eraser(this.grid, 5)

        // necessary procedure to avoid bug
        this.toolsConfig.updatePrevTool(defaultPen)
        this.toolsConfig.updatePrevTool(defaultEraser)

        this.setState({
          loading: false,
          currTool: defaultPen
        })
      }).catch(err => {
        console.error(err)
        console.log(err.message)
        // this must be done because when an error occurs and history is set, initialBoard
        // is not set anymore
        this.getInitialBoard(null)
        this.setState({
          loading: false
        })
        this.props.history.push('/') // change current location programmatically in case of error
      })
  }

  addFavorite (tool) {
    this.setState(() => this.state.favorites.push(tool)) // not needed to change prevState
  }
  removeFavorite = (tool) => {
    this.setState((prevState) => {
      const index = prevState.favorites.indexOf(tool)
      if (index > -1) {
        prevState.favorites.splice(index, 1)
      }
    })
  }
  changeCurrentTool = (tool) => {
    this.toolsConfig.updatePrevTool(this.state.currTool)
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

  refCallback = (ref) => {
    this.canvasContext = ref.canvas.getContext('2d')
  }

  updateBoardId = (id) => {
    console.info('current board id: ' + this.props.match.params.board)
    this.persist.connect(id)
  }

  render () {
    return (
      <div onPaste={this.onPaste} onKeyDown={this.onKeyDown} className={styles.xpto}>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool.bind(this)} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite.bind(this)}
          removeFavorite={this.removeFavorite.bind(this)} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}>
          <Canvas ref={this.refCallback} width={1200} height={800} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} closeModal={this.toggleCleanModal} visible={this.state.showCleanModal} />
        <ShareBoardModal location={this.props.location} history={this.props.history} persist={this.persist} visible={this.state.showShareModal} closeModal={this.toggleShareModal} updateCurrentBoard={this.updateBoardId} />
        <Route path='/signin' component={EnterUserModal} />
        <Loader active={this.state.loading} content='Fetching Data ...' />
      </div>
    )
  }
}
