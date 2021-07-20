import { Button, Col, Row, Spin, Typography } from 'antd';
import React from 'react';
import { configService } from '../services/configService';
import { userService } from '../services/userService';
import { Tile } from './Tile';

const { Title } = Typography;

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

    if (userService.isAuthenticated) {
      if (userService.isAccountReady) {
        this.props.history.push("/");
      } else {
        this.props.history.push("/account/setup");
      }
    }

    this.setState({ initializing: false });
  }

  render() {
    const { initializing } = this.state;

    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col>
          {initializing
            ? <Spin size='large'></Spin>
            :
            <Tile style={{ padding: 16 }}>
              <Title level={4}>Hi! Welcome to <b className='primary-color'>Talk2Me</b></Title>
              <p>In order to use our app you have to login via Zoom</p>
              <Row justify='center' style={{ paddingTop: 16 }}>
                <Button type='default' size='large' onClick={this.redirect}>Login via Zoom</Button>
              </Row>
            </Tile>
          }
        </Col>
      </Row>
    );
  }

  redirect() {
    window.location.href = configService.config.zoomAuthUrl;;
  }
}