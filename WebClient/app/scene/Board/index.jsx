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
import ImportImageModal from './components/Modals/ImportImageModal'

import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Grid from './../../model/Grid'
import {Image} from './../../model/Image'
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
    showImportImageModal: false,
    currTool: defaultPen,
    canvasSize: {width: 0, height: 0},
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

  componentWillMount () {
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
      // TODO(peddavid): This is probably required to be done after state is changed, setState callback or Update
      this.grid.draw(this.canvasContext, 1)
    })
  }

  getInitialBoard (boardId) {
    let persistType = null

    // if there isn't a specific board, or if the user is not authenticated, get persisted data from local storage
    // todo: implement isAuthenticated()
    if (boardId == null) {
      persistType = PersistType.LocalStorage
    } else if (boardId != null) {
      persistType = PersistType.WebSockets
    }

    this.persist = new Persist(persistType, this.canvasContext, this.grid)

    // get initial board from server or from local storage
    this.persist.getInitialBoardAsync(boardId)
      .then(grid => {
        this.grid = grid
        this.persist.grid = this.grid

        if (this.persist.persistType === PersistType.WebSockets) {
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

  // TODO(peddavid): Favorites are not used anywhere here, why is it Board concern? (change to another file?)
  addFavorite = (tool) => {
    this.setState(prevState => ({favorites: [...prevState.favorites, tool]}))
  }
  removeFavorite = (tool) => {
    this.setState(prevState => ({favorites: prevState.favorites.filter(favorite => favorite !== tool)}))
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
    this.setState(prevState => ({showCleanModal: !prevState.showCleanModal}))
  }

  onImageLoad = (imageSrc) => {
    const newImage = new Image({x: 80, y: 80}, imageSrc)
    // do not change this order. image must be added to grid first to set the new id
    this.grid.addImage(newImage)
    newImage.persist(this.persist, this.grid)
    this.grid.draw(this.canvasContext, 1)
  }
  toggleImportImageModal = () => {
    this.setState(prevState => ({showImportImageModal: !prevState.showImportImageModal}))
  }

  toggleShareModal = () => {
    this.setState(prevState => ({showShareModal: !prevState.showShareModal}))
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
      <div>
        <SideBarOverlay grid={this.grid} changeCurrentTool={this.changeCurrentTool} favorites={this.state.favorites} toolsConfig={this.toolsConfig}
          currTool={this.state.currTool} cleanCanvas={this.toggleCleanModal} addFavorite={this.addFavorite}
          removeFavorite={this.removeFavorite} toggleUserModal={this.toggleUserModal} toggleShareModal={this.toggleShareModal}
          onImageLoad={this.toggleImportImageModal} canvasSize={this.state.canvasSize}>
          <Canvas ref={this.refCallback} width={this.state.canvasSize.width} height={this.state.canvasSize.height} {...this.listeners}>
            HTML5 Canvas not supported
          </Canvas>
        </SideBarOverlay>
        <CleanBoardModal cleanCanvas={this.cleanCanvas} onClose={this.toggleCleanModal} open={this.state.showCleanModal} />
        <ImportImageModal onClose={this.toggleImportImageModal} open={this.state.showImportImageModal} onImageLoad={this.onImageLoad} />
        <ShareBoardModal location={this.props.location} history={this.props.history} persist={this.persist} open={this.state.showShareModal} onClose={this.toggleShareModal} updateCurrentBoard={this.updateBoardId} />
        <Route path='/signin' component={EnterUserModal} />
        <Loader active={this.state.loading} content='Fetching Data ...' />
      </div>
    )
  }
}
