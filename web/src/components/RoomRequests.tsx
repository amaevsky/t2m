import React from "react";
import { Col, Row, Typography } from "antd";
import { RoomRequest, roomsService } from "../services/roomsService";
import { RoomCard, RoomCardAction } from "./RoomCard";
import { userService } from "../services/userService";

const { Title } = Typography;

interface State {
  requests: RoomRequest[];
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

  private async acceptRequest(request: RoomRequest) {
    await roomsService.acceptRequest(request.room.id, request.id);
  }

  private async declineRequest(request: RoomRequest) {
    await roomsService.declineRequest(request.room.id, request.id);
  }

  render() {
    const { requests } = this.state;
    const requestsCards = requests
      .map(r => {

        const primary: RoomCardAction[] = r.to.id === userService.user?.id ? [{
          action: () => this.acceptRequest(r),
          title: 'Accept',
        },
        {
          action: () => this.declineRequest(r),
          title: 'Decline'
        }
        ] : [
          {
            action: () => this.declineRequest(r),
            title: 'Cancel'
          }
        ];

        return (
          <Col xl={4} md={6} sm={8} xs={12}>
            <RoomCard room={r.room} type='full' primaryActions={primary} />
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