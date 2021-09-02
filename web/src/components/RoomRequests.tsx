import React from "react";
import { Col, Row, Space, Typography } from "antd";
import { Room, roomsService } from "../services/roomsService";
import { RoomCard, RoomCardAction } from "./RoomCard";

const { Title } = Typography;

interface State {
  requests: Room[];
  loading: boolean;
}

interface Props {

}

export class RoomRequests extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);

    this.state = {
      requests: [],
      loading: true
    };
  }

  async componentDidMount() {
    await this.getData();
  }

  private async getData() {
    const requests = await roomsService.getRequests();
    this.setState({ requests, loading: false });
  }

  private async accept(roomId: string) {
    await roomsService.accept(roomId);
  }

  private async decline(roomId: string) {
    await roomsService.decline(roomId);
  }

  render() {
    const { requests } = this.state;
    const requestsCards = requests
      .map(r => {
        const primary: RoomCardAction[] = [{
          action: () => this.accept(r.id),
          title: 'Accept',
        },
        {
          action: () => this.decline(r.id),
          title: 'Decline'
        }
        ];

        return (
          <Col xl={4} md={6} sm={8} xs={12}>
            <RoomCard room={r} type='full' primaryActions={primary} />
          </Col >
        )
      });

    return (
      <>
        {!!requests.length &&
          <div>
            <Title level={5}>My rooms - Requests</Title>
            <Row gutter={[16, 16]}>
              {requestsCards}
            </Row>
          </div>
        }
      </>
    )
  }
}