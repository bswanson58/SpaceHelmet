using System;
using System.Collections.Generic;
using Fluxor;
using MudBlazor;
using SpaceHelmet.Client.Ui.Actions;

namespace SpaceHelmet.Client.Ui {
    public class AnnouncementHandler : IDisposable {
        private readonly IActionSubscriber  mActionSubscriber;
        private readonly ISnackbar          mNotifier;
        private Snackbar ?                  mNotifications;

        public AnnouncementHandler( IActionSubscriber actionSubscriber, ISnackbar notifier ) {
            mActionSubscriber = actionSubscriber;
            mNotifier = notifier;
        }

        public void StartAnnouncements() {
            mNotifier.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            // Information announcements
            mActionSubscriber.SubscribeToAction<ApiCallStarted>( this, action => {
                if( mNotifications?.Severity == Severity.Info ) {
                    mNotifier.Remove( mNotifications );

                    mNotifications = null;
                }

                mNotifications = mNotifier.Add<ApiCallAnnouncement>( 
                    new Dictionary<string, object>() {
                        { nameof( ApiCallAnnouncement.AnnouncementMessage ), action.AnnouncementMessage }
                    },
                    Severity.Info, 
                    config => {
                        config.ActionColor = Color.Info;
                        config.HideIcon = true;
                        config.ShowCloseIcon = false;
                        config.RequireInteraction = false;
                        config.VisibleStateDuration = (int)TimeSpan.FromSeconds( 60 ).TotalMilliseconds;
                    });
            } );

            mActionSubscriber.SubscribeToAction<ApiCallCompleted>( this, _ => {
                if( mNotifications?.Severity == Severity.Info ) {
                    mNotifier.Remove( mNotifications );

                    mNotifications = null;
                }
            } );

            // API failure announcements
            mActionSubscriber.SubscribeToAction<ApiCallFailure>( this, action => {
                if( mNotifications != null ) {
                    mNotifier.Remove( mNotifications );

                    mNotifications = null;
                }

                mNotifications = mNotifier.Add<ErrorAnnouncement>(
                    new Dictionary<string, object>() {
                        { nameof( ErrorAnnouncement.ErrorMessage ), action.Message }
                    },
                    Severity.Error,
                    config => {
                        config.ActionColor = Color.Error;
                        config.HideIcon = true;
                        config.ShowCloseIcon = true;
                        config.RequireInteraction = false;
                        config.VisibleStateDuration = (int)TimeSpan.FromSeconds( 60 ).TotalMilliseconds;
                    } );
            } );
        }

        public void Dispose() {
            mActionSubscriber.UnsubscribeFromAllActions( this );
        }
    }
}
