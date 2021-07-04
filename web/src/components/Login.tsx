import { Button, Col, Row } from 'antd';
import axios from 'axios';
import React from 'react';

export class Login extends React.Component {

  render() {
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>
          <Button onClick={this.redirect}>Login</Button>
        </Col>
      </Row>
    );
  }

  async redirect() {
    const response = await axios.get<string>('https://localhost:44361/config/zoom');
    window.location.href = response.data;
  }
}