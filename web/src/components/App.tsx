import React from 'react';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { FindRooms } from './FindRooms';
import { MyRooms } from './MyRooms';
import { IHasBreakpoint } from '../utilities/withBreakpoints';
import { Header } from './Header';

interface Props extends IHasBreakpoint, RouteComponentProps {

}

export default class App extends React.Component<Props> {

  render() {
    return (
      <>
        <Header />
        <div style={{ padding: '0 10px' }}>
          <Switch>
            <Route path="/rooms/my" component={MyRooms} />
            <Route path="/" component={FindRooms} />
          </Switch>
        </div>
      </>
    );
  }
}