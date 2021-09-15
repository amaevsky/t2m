import { BulbOutlined } from "@ant-design/icons";
import { Button, Col, Row, Space, Typography } from "antd";
import { useHistory } from "react-router";
import { routes } from "./App";

export const WelcomeTutorial = () => {
  const history = useHistory();

  return (
    <Row style={{ flex: 1 }} align='middle' justify='center'>
      <Space direction='vertical' size={40}>
        <div></div>
        <Row justify='center'>
          <Typography.Title level={3}>You are all set! This is how it works:</Typography.Title>
        </Row>
        <Row gutter={[32, 32]} justify='center'>
          <Col lg={5} md={6} xs={16} >
            <img src="/Tutorial - Step1.png" width='100%' />
          </Col >
          <Col lg={5} md={6} xs={16}>
            <img src="/Tutorial - Step2.png" width='100%' />
          </Col>
          <Col lg={5} md={6} xs={16}>
            <img src="/Tutorial - Step3.png" width='100%' />
          </Col>
        </Row>
        <Row justify='center'>
          <Space size='middle'>
            <BulbOutlined className='primary-color' style={{ fontSize: 26 }} />
            <span>
              Tip: When you create a room, create it for a day or two in advance, so other user can see it and enter it.
              You can plan your week and create multiple rooms in advance as well.
            </span>
          </Space>
        </Row>
        <Row justify='center'>
          <Button
            onClick={() => history.push(routes.default)} style={{ padding: '0 40px' }}
            type='primary'
            size='large'>
            Start my journey
          </Button>
        </Row>
      </Space>
    </Row>
  );
}