import React from 'react'
import SignInModal from './components/SignInModal'
import SignUpModal from './components/SignUpModal'

export default class EnterUserModal extends React.Component {
  state = {
    classes: '',
    SignInModalToggle: true
  }

  flip = () => {
    this.setState(prev => {
      return {
        SignInModalToggle: !prev.SignInModalToggle
      }
    })
  }

  onClose = () => {
    this.props.history.push('/') // change current location programmatically
  }

  render () {
    const signInModal = this.state.SignInModalToggle
    return (
      <div className={this.state.classes}>
        <SignInModal toggleUserModal={this.onClose} register={this.flip} open={signInModal} />
        <SignUpModal toggleUserModal={this.onClose} login={this.flip} open={!signInModal} />
      </div>
    )
  }
}
