import React from "react";

type Props = {
  auction: any;
};

export default function AuctionCard(props: Props) {
  return <div>{props.auction.make}</div>;
}
