import React from 'react'
import { Modal, Button, Icon, Input, Message } from 'semantic-ui-react'

export default class AddBoardModal extends React.Component {
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

    this.setState({
      loading: true
    })

    this.props.addBoardAsync(this.boardName)
      .then(board => {
        this.setState({
          loading: false,
          error: false
        })
      }).catch(err => {
        console.log(err)
        this.setState({
          loading: false,
          error: true
        })
      })
  }

  render () {
    const modalHeader = (
      <Modal.Header>
        <Icon name='save' size='small' />
        Add New Board
      </Modal.Header>
    )

    if (this.props.auth.isAuthenticated()) {
      return (
        <Modal size='small' open={this.props.visible} onClose={this.onClose}>
          {modalHeader}
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
          {modalHeader}
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
