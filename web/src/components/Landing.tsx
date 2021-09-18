import { Col, Row, Button, Typography, Space } from "antd";
import { Redirect } from "react-router-dom";
import { userService } from "../services/userService";
import { routes } from "./App";
const { Title } = Typography;

export const Landing = (props: any) => {

  const signinBtn =
    <Button
      onClick={() => props.history.push(routes.login.login)} style={{ width: 150 }}
      type='primary'
      size='large'>
      Get Started
    </Button>;

  return (
    <>
      {userService.isAccountReady &&
        <Redirect to={routes.app.findRoom} />
      }
      <Row align='middle'>
        <Col md={14} xs={24} style={{ padding: '100px 0' }}>

          <Row justify='center'>
            <Col span={18}>
              <Space direction='vertical' size={40}>

                <img height='120' src='talk2me-unfilled.png'></img>

                <Title level={3}>Improve any language by speaking with people who have the same level of language. For Free.</Title>

                <ul style={{ fontSize: 14 }}>
                  <li>connect with people with the same goal</li>
                  <li>overcome the fear of speaking</li>
                  <li>save some money on tutors</li>
                </ul>
                {signinBtn}
              </Space>
            </Col>
          </Row>
        </Col>
        <Col md={10} xs={24}>
          <img src='landing.png' width='100%'></img>
        </Col>
      </Row >

      <Space direction='vertical' size={40}>
        <div></div>
        <Row justify='center'>
          <Title level={3}>How it works</Title>
        </Row>
        <Row gutter={[32, 32]} justify='center'>
          <Col lg={5} md={6} xs={16} >
            <img src="Tutorial - Step1.png" width='100%' />
          </Col >
          <Col lg={5} md={6} xs={16}>
            <img src="Tutorial - Step2.png" width='100%' />
          </Col>
          <Col lg={5} md={6} xs={16}>
            <img src="Tutorial - Step3.png" width='100%' />
          </Col>
        </Row>
        <Row justify='center'>
          {signinBtn}
        </Row>
        <div></div>
      </Space>

    </>
  )
}