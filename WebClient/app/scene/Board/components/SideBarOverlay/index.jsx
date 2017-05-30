// @flow

import React from 'react'
import {
  Sidebar,
  Segment,
  Button,
  Menu,
  Icon
} from 'semantic-ui-react'

export default class SideBarOverlay extends React.Component {
  state = {visible: false}
  toggleVisibility = () => this.setState({ visible: !this.state.visible })

  render () {
    const { visible } = this.state
    return (
      <div>
        <Button onClick={this.toggleVisibility}>Toggle Visibility</Button>
        <Sidebar.Pushable as={Segment}>
          <Sidebar as={Menu} animation='overlay' direction='top' visible={visible} inverted>
            <Menu.Item name='home'>
              <Icon name='home' />
              Home
            </Menu.Item>
            <Menu.Item name='about'>
              <Icon name='bold' />
              About
            </Menu.Item>
          </Sidebar>
          <Sidebar.Pusher>
            <Segment basic >
              {this.props.children}
            </Segment>
          </Sidebar.Pusher>
        </Sidebar.Pushable>
      </div>
    )
  }
}
