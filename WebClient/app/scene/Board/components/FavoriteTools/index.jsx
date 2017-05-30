// @flow

import React from 'react'

import { Grid, Dropdown, Button } from 'semantic-ui-react'

export default class FavoriteTools extends React.Component {
  render () {
    return (
      <div>
        <Grid divided textAlign='center'>
          <Grid.Row columns={this.props.favorites.length}>
            {this.props.favorites.map(favorite => (
              <Grid.Column>
                <Dropdown trigger={<Button circular icon='angle down' />} icon={null} upward>
                  <Dropdown.Menu>
                    <Dropdown.Item as={Button} />
                  </Dropdown.Menu>
                </Dropdown>
              </Grid.Column>
            ))}
          </Grid.Row>
        </Grid>
      </div>
    )
  }
}
