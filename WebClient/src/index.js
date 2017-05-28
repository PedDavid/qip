// @flow

import React from 'react'
import { render } from 'react-dom'

function Hello ({name}: {name: string}) {
  return <div>Hello {name}</div>
}

render(
  <Hello name='Pedro' />,
  document.getElementById('root')
)
