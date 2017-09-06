import React from 'react'
import { Modal, Icon, Form, Checkbox, Label, Button, Divider } from 'semantic-ui-react'
import styles from './../../styles.scss'
import SocialLogin from './components/SocialLogin'

export default class SignInModal extends React.Component {
  state = {
    classes: ''
  }

  flip = () => {
    this.setState(prev => {
      if (prev.classes === styles.flip) {
        return {classes: styles.flipInverse}
      } else {
        return {classes: styles.flip}
      }
    })

    setTimeout(() => {
      this.setState({classes: styles.flipInverse}) // this will trigger update one more time but this way modal is not flipped when showed the 2nd time
      this.props.register()
    }, 400)
  }

  render () {
    return (
      <Modal onClose={this.props.toggleUserModal} className={this.state.classes} size='small' open={this.props.open}>
        <Modal.Header>
          <Icon name='user' />
          Login
        </Modal.Header>
        <Modal.Content>
          <Form>
            <Form.Field>
              <label>username</label>
              <input placeholder='username' />
            </Form.Field>
            <Form.Field>
              <label>password</label>
              <span>
                <input type='password' placeholder='password' />
              </span>
            </Form.Field>
            <Form.Field>
              <Checkbox label='I agree to the Terms and Conditions' />
            </Form.Field>
            <Button.Group>
              <Button primary type='submit'> Login </Button>
              <Button.Or />
              <Button secondary type='button' onClick={this.flip}> Register </Button>
            </Button.Group>
          </Form>
          <Divider horizontal>Or</Divider>
          <Label> Login With ... </Label>
          <SocialLogin />
        </Modal.Content>
      </Modal>
    )
  }
}
