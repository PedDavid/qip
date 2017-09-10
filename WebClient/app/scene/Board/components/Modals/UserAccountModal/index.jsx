import React from 'react'
import { Modal, Header, Image } from 'semantic-ui-react'

export default class UserAccountModal extends React.Component {
  render () {
    if (this.props.auth.isAuthenticated()) {
      const user = this.props.auth.tryGetProfile()
      return (
        <Modal open={this.props.visible} onClose={this.props.closeModal}>
          <Modal.Header>{user.name}</Modal.Header>
          <Modal.Content image>
            <Image wrapped size='medium' src={user.picture} />
            <Modal.Description>
              <Header>Nickname</Header>
              <p>{user.nickname}</p>
              <Header>Link Accounts</Header>
              <p>Link your account to others</p>
              <p>... Implement ...</p>
            </Modal.Description>
          </Modal.Content>
        </Modal>
      )
    } else {
      return null
    }
  }
}
