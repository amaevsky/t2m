import { Button, Col, Row } from 'antd';
import axios from 'axios';
import React from 'react';
import { Redirect } from 'react-router-dom';
import { userService } from '../services/userService';

interface State {
  initializing: boolean
}
export class Login extends React.Component<any, State> {

  constructor(props: any) {
    super(props);

    this.state = { initializing: true };
  }

  async componentDidMount() {
    try {
      await userService.initialize();
    } catch {

    }

    this.setState({ initializing: false })
  }

  render() {
    const { initializing } = this.state;

    if (userService.user) {
      return <Redirect to="/" />
    }

    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>
          {initializing
            ? 'Login is in progress...'
            : <Button type='primary' size='large' onClick={this.redirect}>Login</Button>
          }
        </Col>
      </Row>
    );
  }

  async redirect() {
    const response = await axios.get<string>('https://localhost:44361/config/zoom');
    window.location.href = response.data;
  }
}