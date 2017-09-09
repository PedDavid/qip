import React from 'react'
import { Modal, Button, Icon, Input, Message, Divider, Dropdown, Popup } from 'semantic-ui-react'

const baseUrl = 'http://localhost:8080/board/'

export default class ShareBoardModal extends React.Component {
  users = []
  usersPermission = 1
  state = {
    boardUrl: this.props.persist.boardId != null ? baseUrl + this.props.persist.boardId : '',
    loading: false,
    loadingShareWUser: false,
    error: false,
    currUser: '',
    publicVisibilityView: this.props.currentBoard.basePermission === 1,
    publicVisibilityEdit: this.props.currentBoard.basePermission === 2,
    usersVisibilityView: true,
    usersVisibilityEdit: false,
    publicVisibilityLoading: false,
    userVisibilityLoading: false,
    selectedUsers: []
  }
  onClose = () => {
    this.props.closeModal()
  }

  getBoard = () => {
    if (this.state.boardUrl !== '' && !this.state.error) {
      return
    }

    // TODO: POST is not idempotent, "sharing" a board multiple times should not be able to produce side effects
    this.props.addBoardAsync(`Local Board`)
      .then(insertedBoard => {
        // this.props.updateCurrentBoard(board.id) // todo (simaovii): verificar se isto é necessário
        this.setState({
          boardUrl: baseUrl + insertedBoard.id,
          loading: false,
          error: false
        })
        this.props.history.push('/board/' + insertedBoard.id)
      }).catch(err => {
        console.log(err)
        this.setState({
          boardUrl: 'Error: Please contact support',
          loading: false,
          error: true
        })
      })

    this.setState({
      loading: true
    })
  }

  changePublicVisibility = (props) => {
    this.setState({
      publicVisibilityLoading: true
    })
    const btn = props[0]
    let updatedPublicVisibilityEdit = false
    let updatedPublicVisibilityView = false
    let updatedPermission
    if (btn === 'edit') {
      updatedPublicVisibilityEdit = !this.state.publicVisibilityEdit
      updatedPermission = updatedPublicVisibilityEdit ? 2 : 0
    } else if (btn === 'view') {
      updatedPublicVisibilityView = !this.state.publicVisibilityView
      updatedPermission = updatedPublicVisibilityView ? 1 : 0
    }

    this.props.persist
      .updateBoardBasePermission(this.props.currentBoard.id, this.props.currentBoard.name, updatedPermission, this.props.auth.getAccessToken())
      .then(users => {
        this.setState({
          publicVisibilityEdit: updatedPublicVisibilityEdit,
          publicVisibilityView: updatedPublicVisibilityView,
          publicVisibilityLoading: false
        })
      }).catch(err => {
        this.setState({
          publicVisibilityLoading: false
        })
        console.error(err)
      })
  }

  onChangeUser = (e, { value }) => this.setState({ selectedUsers: value })

  changeUsersVisibility = () => {
    this.usersPermission = this.state.usersVisibilityView ? 2 : 1 // it is with opposite values because they going to swap

    this.setState(prev => {
      return {
        usersVisibilityView: prev.usersVisibilityEdit,
        usersVisibilityEdit: !prev.usersVisibilityEdit
      }
    })

    console.log(this.usersPermission)
  }

  shareWithUsers = () => {
    this.props.persist.createUsersPermission(this.state.selectedUsers, this.props.currentBoard.id, this.usersPermission, this.props.auth.getAccessToken())
      .then(allRes => {
        this.setState({
          userVisibilityLoading: false,
          selectedUsers: []
        })
        this.value = []
      }).catch(err => {
        this.setState({
          userVisibilityLoading: false
        })
        console.error(err)
      })

    this.setState({
      userVisibilityLoading: true
    })
  }

  componentWillReceiveProps (nextProps) {
    const boardId = this.state.boardUrl
    if (this.state.boardUrl !== '' && !this.state.error) {
      return
    }
    if (boardId !== baseUrl + nextProps.persist.boardId || this.props.currentBoard.basePermission !== nextProps.currentBoard.basePermission) {
      this.setState({
        boardUrl: nextProps.persist.boardId != null ? baseUrl + nextProps.persist.boardId : '',
        publicVisibilityView: this.props.currentBoard.basePermission === 1,
        publicVisibilityEdit: this.props.currentBoard.basePermission === 2
      })
    }
  }

  componentWillMount = () => {
    this.getUsers()
  }
  componentWillUpdate = () => {
    this.getUsers()
  }
  getUsers = () => {
    if (!this.props.auth.isAuthenticated()) {
      return
    }
    this.props.persist.getUsersAsync(this.props.auth.getAccessToken())
      .then(users => {
        const profile = this.props.auth.tryGetProfile()
        profile !== null && (this.users = users.filter(user => user.id !== profile.sub))
      })
  }

  render () {
    let getUrlBtnDisabled = false
    if (this.state.boardUrl !== '' && !this.state.error) {
      getUrlBtnDisabled = true
    }
    const isNotAuthenticated = !this.props.auth.isAuthenticated()

    return (
      <Modal size='small' open={this.props.visible} onClose={this.onClose}>
        <Modal.Header>
          <Icon name='share' size='small' />
          Share Your Board
        </Modal.Header>
        <Modal.Content>
          <div>
            <p> Visibility to public </p>
            <span style={{paddingBottom: '10px'}}>
              <Button disabled={isNotAuthenticated} toggle icon loading={this.state.publicVisibilityLoading} active={this.state.publicVisibilityEdit} onClick={this.changePublicVisibility.bind(this, ['edit'])} ><Icon name='edit' /></Button>
              <Button disabled={isNotAuthenticated} toggle icon loading={this.state.publicVisibilityLoading} active={this.state.publicVisibilityView} onClick={this.changePublicVisibility.bind(this, ['view'])} ><Icon name='eye' /></Button>
            </span>
          </div>
          <Divider />
          <div>
            <p>Share Your Board URL to a specific friend by username</p>
            <Dropdown disabled={isNotAuthenticated} placeholder='Search Users' fluid multiple search selection
              options={this.users.map(user => { return {key: user.name, text: user.name, value: user.id} })}
              loading={false} value={this.state.selectedUsers} onChange={this.onChangeUser} />
            <div style={{paddingTop: '10px', paddingBottom: '10px'}}>
              <Button toggle active={this.state.usersVisibilityEdit} disabled={isNotAuthenticated} onClick={this.changeUsersVisibility} icon><Icon name='edit' /></Button>
              <Button toggle active={this.state.usersVisibilityView} disabled={isNotAuthenticated} onClick={this.changeUsersVisibility} icon><Icon name='eye' /></Button>
              <Popup
                trigger={
                  <Button loading={this.state.userVisibilityLoading} onClick={this.shareWithUsers} disabled={isNotAuthenticated} icon style={{ marginLeft: '30px' }}><Icon name='share' /></Button>
                }
                content='To share your board with users you must be authenticated!.'
                basic
              />
            </div>
          </div>
          <Divider />
          <div>
            <Message hidden={!isNotAuthenticated} negative size='tiny'>
              <Message.Header> <Icon name='warning' /> Be careful!</Message.Header>
              <p>If you get the URL while not authenticated all people with access to this url can write and read your board and you can never change that.</p>
              <p>To be able to change public permission, please sign up!</p>
            </Message>
            <p>Get the URL to Share Your Board By A URL</p>
            <Button disabled={getUrlBtnDisabled} style={{float: 'left'}} loading={this.state.loading}
              onClick={this.getBoard} size='medium'>
              Get Url
            </Button>
            <Input readOnly fluid error={this.state.error}
              action={{ color: 'teal', icon: 'copy' }}
              placeholder={this.state.boardUrl} />
          </div>
        </Modal.Content>
      </Modal>
    )
  }
}
