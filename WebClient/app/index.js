import React from 'react'
import { render } from 'react-dom'
import Board from './scene/Board'
import {
  BrowserRouter as Router,
  Route,
  Switch
} from 'react-router-dom'

render(
  <div>
    <Router>
      <Switch> {/* Renders the first path that match */}
        <Route path='/:board' component={Board} />
        <Route path='/' component={Board} />
      </Switch>
    </Router>
  </div>,
  document.getElementById('root')
)
