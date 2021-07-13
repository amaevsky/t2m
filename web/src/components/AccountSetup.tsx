
import React from 'react';
import { Col, Row } from 'antd';
import { UserProfileEdit } from './UserProfileEdit';
export class AccountSetup extends React.Component<any> {

  render() {
    return (
      <Row align='middle' justify='center' style={{ minHeight: '100vh' }}>
        <Col span={10}>
          <UserProfileEdit afterSave={() => this.props.history.push("/")} />
        </Col>
      </Row >
    );
  }
}