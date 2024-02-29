use crate::models::messages::PolledMessage;
use std::fmt::Debug;

/// The trait represent the logic responsible for handling the message and is used by the `IggyClient`.
pub trait MessageHandler: Send + Sync + Debug {
    fn handle(&self, message: PolledMessage);
}
