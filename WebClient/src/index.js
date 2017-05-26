import React from 'react'
import { render } from 'react-dom'

function Hello () {
  return React.createElement('div', null, `Hello webpack World`)
}

render(
  React.createElement(Hello, null, null),
  document.getElementById('root')
)
