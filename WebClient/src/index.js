import React from 'react'
import { render } from 'react-dom'

function Hello () {
  return <div>Hello React JSX World! powered by Babel</div>
}

render(
  <Hello />,
  document.getElementById('root')
)
