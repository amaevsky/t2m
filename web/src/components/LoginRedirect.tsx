import { Col, Row, Spin } from 'antd';
import React from 'react';
import { authService } from '../services/authService';
import { userService } from '../services/userService';
import { routes } from './App';

export class LoginRedirect extends React.Component<any> {

  async componentDidMount() {
    const code = new URLSearchParams(window.location.search).get('code') as string;
    window.location.href = `app.t2m://oauth_callback?code=${code}`;
  }

  render() {
    return (
      <Row style={{ flex: 1 }} align='middle' justify='center'>
        <Col><Spin size='large' /></Col>
      </Row>
    );
  }
}