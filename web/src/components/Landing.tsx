import { Col, Row, Button, Typography, Space } from "antd"
const { Title } = Typography;

export const Landing = (props: any) => {
  return (
    <Row align='middle'>
      <Col md={12} xs={24} style={{ padding: '100px 0' }}>

        <Row justify='center'>
          <Col span={18}>
            <Space direction='vertical' size={40}>

              <img height='120' src='/talk2me-unfilled.png'></img>

              <Title level={3}>Improve any language by speaking with people who have the same level of language. For Free.</Title>

              <ul style={{ fontSize: 14 }}>
                <li>connect with people with the same goal</li>
                <li>overcome the fear of speaking</li>
                <li>save some money on tutors</li>
              </ul>

              <Button onClick={() => props.history.push('/login')} style={{ width: 150 }} type='primary' size='large'>Get Started</Button>

            </Space>
          </Col>
        </Row>
      </Col>
      <Col md={12} xs={24}>
        <img src='/landing.png' width='100%'></img>
      </Col>
    </Row >
  )
}