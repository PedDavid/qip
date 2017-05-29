// @flow

import React from 'react'
import { render } from 'react-dom'

import Canvas from './components/Canvas'

function Hello ({name}: {name: string}) {
  return <div>Hello {name}</div>
}

render(
  <div>
    <Hello name='Pedro' />
    <Canvas width={300} height={300} onDown={evt => {}}>
      HTML5 Canvas not supported
    </Canvas>
  </div>,
  document.getElementById('root')
)
