import React from 'react'
import { Modal, Icon, Form, Checkbox, Button } from 'semantic-ui-react'
import styles from './../../styles.scss'

export default class SignUpModal extends React.Component {
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
      this.props.login()
    }, 400)
  }

  render () {
    return (
      <Modal onClose={this.props.toggleUserModal} className={this.state.classes} size='small' open={this.props.open} closeIcon>
        <Modal.Header>
          <Icon name='user' />
          Register
        </Modal.Header>
        <Modal.Content>
          <Form>
            <Form.Field>
              <label>First Name</label>
              <input placeholder='First Name' />
            </Form.Field>
            <Form.Field>
              <label>Last Name</label>
              <input placeholder='Last Name' />
            </Form.Field>
            <Form.Field>
              <label>Password</label>
              <input placeholder='Password' />
            </Form.Field>
            <Form.Field>
              <label>Email</label>
              <input placeholder='Email' />
            </Form.Field>
            <Form.Field>
              <Checkbox label='I agree to the Terms and Conditions' />
            </Form.Field>
            <Button.Group>
              <Button primary type='submit'> Register </Button>
              <Button.Or />
              <Button secondary type='button' onClick={this.flip}> Login </Button>
            </Button.Group>
          </Form>
        </Modal.Content>
      </Modal>
    )
  }
}
