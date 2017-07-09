// @flow

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

  render () {
    const signInModal = this.props.visible && this.state.SignInModalToggle
    const signUpModal = this.props.visible && !this.state.SignInModalToggle

    return (
      <div className={this.state.classes}>
        <SignInModal toggleUserModal={this.props.toggleUserModal} register={this.flip} visible={signInModal} />
        <SignUpModal toggleUserModal={this.props.toggleUserModal} login={this.flip} visible={signUpModal} />
      </div>
    )
  }
}
