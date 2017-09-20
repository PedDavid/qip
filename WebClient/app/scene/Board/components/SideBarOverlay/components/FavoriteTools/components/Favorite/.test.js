import React from 'react'
import { shallow, mount } from 'enzyme'

import Favorite from '.'

import { Button } from 'semantic-ui-react'

import Pen from './../../../../../../../../model/tools/Pen'

import ToolsConfig from './../../../../../../../../model/ToolsConfig'
import defaultToolsConfig from './../../../../../../../../public/configFiles/defaultTools'


describe('Component: Favorite', () => {
  const tool = new Pen(null, 'red', 5)
  const toolsConfig = new ToolsConfig(defaultToolsConfig)
  const props = {
    fav: tool,
    currTool: tool,
    toolsConfig
  }
  const wrapper = shallow(<Favorite {...props} />)
  it('Is not null', () => {
    expect(wrapper).not.toBeNull()
  })
  it('Is instance of Favorite', () => {
    expect(wrapper.instance()).toBeInstanceOf(Favorite)
  })
  it('Type is <div>', () => {
    expect(wrapper.type()).toBe('div')
  })
  it('Props are delivered', () => {
    expect(wrapper.instance().props).toMatchObject(props)
  })
  it('Renders as expected', () => {
    expect(wrapper.html()).toMatchSnapshot()
  })
  it('Has 2 buttons', () => {
    expect(wrapper.find(Button)).toHaveLength(2)
  })
  it.skip('Check type of tool property', () => {})
  it('State starts with property openFavMenu as false', () => {
    const wrapper = shallow(<Favorite {...props} />)
    expect(wrapper.state().openFavMenu).toBe(false)
  })
  it('Listens to onContextMenu', () => {
    const wrapper = shallow(<Favorite {...props} />)
    wrapper.simulate('contextMenu', {preventDefault: () => {}})
    expect(wrapper.state().openFavMenu).toBe(true)
  })
  it('Listens to buttons onClicks, has changeCurrentTool and removeFavorite listeners', () => {
    expect.assertions(2)
    const mProps = {
      ...props,
      changeCurrentTool: fav => {
        expect(fav).toBe(props.fav)
      },
      removeFavorite: fav => {
        expect(fav).toBe(props.fav)
      }
    }
    const wrapper = shallow(<Favorite {...mProps} />)
    wrapper.find(Button).forEach(buttonWrapper => buttonWrapper.simulate('click'))
  })
})