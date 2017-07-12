// @flow

import React from 'react'
import SignInModal from './components/SignInModal'
import SignUpModal from './components/SignUpModal'

export default class EnterUserModal extends React.Component {

  onClose = () => {
    this.props.history.push('/') // change current location programmatically
  }

  render () {
    return (
      <div className={this.state.classes}>
        <Modal size={size} open={open} onClose={this.onClose}>
          <Modal.Header>
            <Icon name='share' size='small'/>
            Share Your Board
          </Modal.Header>
          <Modal.Content>
            <p>Are you sure you want to delete your account</p>
          </Modal.Content>
          <Modal.Actions>
            <Button negative>
              No
            </Button>
            <Button positive icon='checkmark' labelPosition='right' content='Yes' />
          </Modal.Actions>
        </Modal>
      </div>
    )
  }
}
