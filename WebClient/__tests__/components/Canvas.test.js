import React from 'react'
import { shallow, mount } from 'enzyme'

import Canvas from '../../src/components/Canvas'

describe('Component: Canvas', () => {
  it('Is not null', () => {
    const wrapper = shallow(<Canvas />)
    expect(wrapper).not.toBeNull()
  })
  it('Is instance of Canvas', () => {
    const wrapper = shallow(<Canvas />)
    expect(wrapper.instance()).toBeInstanceOf(Canvas)
  })
  it('Type is <canvas>', () => {
    const wrapper = shallow(<Canvas />)
    expect(wrapper.type()).toBe('canvas')
  })
  it('Props are delivered', () => {
    const props = { width: 1, height: 1 }
    const wrapper = shallow(<Canvas {...props} />)
    expect(wrapper.instance().props).toMatchObject(props)
  })
  // Otherwise browsers throw warnings because prop is unknown to <canvas>
  it('Does not pass listeners to <canvas>', () => {
    const props = { width: 1, height: 1 }
    const listeners = { onDown: evt => {} }
    const wrapper = shallow(<Canvas {...props} {...listeners} />)
    expect(wrapper.instance().props).toHaveProperty('onDown')
    expect(wrapper.props()).not.toHaveProperty('onDown')
  })
  it('Mounts', () => {
    const wrapper = mount(<Canvas />)
    expect(wrapper).not.toBeNull()
  })
})
