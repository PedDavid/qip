// @flow

import 'semantic-ui-css/semantic.min.css'
import React from 'react'
import { render } from 'react-dom'

import { Header, Sidebar, Segment, Button, Menu, Icon } from 'semantic-ui-react'

import Canvas from './scene/Board/components/Canvas'

class SideBarOverlay extends React.Component {
  state = {visible: false}
  toggleVisibility = () => this.setState({ visible: !this.state.visible })

  render () {
    const { visible } = this.state
    return (
      <div>
        <Button onClick={this.toggleVisibility}>Toggle Visibility</Button>
        <Sidebar.Pushable as={Segment}>
          <Sidebar as={Menu} animation='overlay' width='thin' visible={visible} icon='labeled' vertical inverted>
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

render(
  <div>
    <SideBarOverlay>
      <Header as='h1'>Hello Semantic UI</Header>
      <Canvas width={300} height={300}>
        HTML5 Canvas not supported
      </Canvas>
    </SideBarOverlay>
  </div>,
  document.getElementById('root')
)
