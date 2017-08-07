import React from 'react'
import { Modal, Button, Icon, Input, Message } from 'semantic-ui-react'
import fetch from 'isomorphic-fetch'

const baseUrl = 'http://localhost:57059/'
const boardsPath = 'api/boards/'

export default class SaveBoardModal extends React.Component {
  state = {
    loading: false,
    error: false
  }
  onClose = () => {
    this.props.closeModal()
  }

  onNameChange = (event, data) => {
    this.boardName = data.value
  }

  // todo: this first Promise can be merged with ShareBoardModal
  saveBoard = () => {
    if (!this.boardName) {
      this.setState({
        loading: false,
        error: true
      })
      return
    }
    const savePromise = new Promise((resolve, reject) => {
      if (this.props.persist.connected) {
        resolve(this.props.persist.boardId)
      } else {
        fetch(`${baseUrl}${boardsPath}`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          method: 'POST',
          body: JSON.stringify({
            'name': this.boardName,
            'maxDistPoints': 0
          })
        }).then(response => {
          if (response.status >= 400) {
            throw new Error('Bad response from server')
          }
          return response.json()
        }).then(board => {
          this.props.updateCurrentBoard(board.id)
          this.props.history.push('/board/' + board.id)
          resolve(board)
        }).catch(err => {
          reject(err)
        })
      }
    })

    savePromise
      .then(board => {
        const addBoardToUserFetch = fetch(`${baseUrl}${boardsPath}${board.id}/usersboards`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          },
          method: 'POST',
          body: JSON.stringify({
            'userId': this.props.auth.tryGetProfile().sub,
            'boardId': board.id,
            'permission': 0
          })
        })

        this.setState({
          loading: false,
          error: false
        })

        return addBoardToUserFetch
      }).catch(err => {
        console.log(err)
        this.setState({
          loading: false,
          error: true
        })
      })

    this.setState({
      loading: true
    })
  }

  render () {
    if (this.props.auth.isAuthenticated()) {
      return (
        <Modal size='small' open={this.props.visible} onClose={this.onClose}>
          <Modal.Header>
            <Icon name='save' size='small' />
            Save Your Board
          </Modal.Header>
          <Modal.Content>
            <p>Board Name</p>
            <Input error={this.state.error} onChange={this.onNameChange} fluid placeholder='Enter Board Name' />
            <Button style={{float: 'left', margin: '7px'}} loading={this.state.loading}
              onClick={this.saveBoard} size='medium'>
              Save
            </Button>
          </Modal.Content>
        </Modal>
      )
    } else {
      return (
        <Modal size='small' open={this.props.visible} onClose={this.onClose}>
          <Modal.Header>
            <Icon name='save' size='large' />
            Save Your Board
          </Modal.Header>
          <Modal.Content>
            <Message
              warning
              size='small'
              icon='warning'
              header='You Must be authenticated to save your board!'
              content='Please Login or Register First'
            />
            <Button style={{float: 'left', margin: '7px'}}
              onClick={this.props.auth.login} size='medium'>
              Sign Up
            </Button>
          </Modal.Content>
        </Modal>
      )
    }
  }
}
