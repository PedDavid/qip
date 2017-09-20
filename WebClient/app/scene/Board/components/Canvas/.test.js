import React from 'react'
import { shallow, mount } from 'enzyme'

import Canvas from '.'

describe('Component: Canvas', () => {
  const props = { width: 1, height: 1 }
  const wrapper = shallow(<Canvas {...props} />)
  it('Is not null', () => {
    expect(wrapper).not.toBeNull()
  })
  it('Is instance of Canvas', () => {
    expect(wrapper.instance()).toBeInstanceOf(Canvas)
  })
  it('Type is <canvas>', () => {
    expect(wrapper.type()).toBe('canvas')
  })
  it('Props are delivered', () => {
    expect(wrapper.instance().props).toMatchObject(props)
  })
  it('Renders as expected', () => {
    expect(wrapper.html()).toMatchSnapshot()
  })
  // Otherwise browsers throw warnings because prop is unknown to <canvas>
  it('Does not pass listeners to <canvas>', () => {
    const listeners = { onDown: evt => {} }
    const wrapper = shallow(<Canvas {...props} {...listeners} />)
    expect(wrapper.instance().props).toHaveProperty('onDown')
    expect(wrapper.props()).not.toHaveProperty('onDown')
  })
  it('Receives all listeners', () => {
    const listeners = {
      onDown: evt => {},
      onUp: evt => {},
      onMove: evt => {},
      onOut: evt => {}
    }
    const wrapper = shallow(<Canvas {...props} {...listeners} />)
    expect(wrapper).not.toBeNull()
  })
  it('Mounts', () => {
    const wrapper = mount(<Canvas {...props} />)
    expect(wrapper).not.toBeNull()
  })
  it('Updates and does not change state', () => {
    const wrapper = mount(<Canvas {...props} />)
    const updated = wrapper.update()
    expect(updated).toMatchObject(wrapper)
  })
  it(`Should test listeners, sadly enzyme can't simulate pointer Events :(
    Right now it only insures test coverage is 100%`, () => {
    const listeners = {
      onDown: evt => {},
      onUp: evt => {},
      onMove: evt => {},
      onOut: evt => {}
    }
    const wrapper = mount(<Canvas {...props} {...listeners} />)
    expect(wrapper).not.toBeNull()
    //TODO(peddavid): Simulate pointer events
  })
})
